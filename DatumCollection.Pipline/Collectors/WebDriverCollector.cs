﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatumCollection.Configuration;
using DatumCollection.Infrastructure.Abstraction;
using DatumCollection.Infrastructure.Spider;
using DatumCollection.Infrastructure.Web;
using DatumCollection.MessageQueue;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Safari;
using OpenQA.Selenium.Support.UI;

namespace DatumCollection.Pipline.Collector
{
    public class WebDriverCollector : ICollector
    {
        private readonly ILogger<WebDriverCollector> _logger;
        private readonly IMessageQueue _mq;
        private readonly SpiderClientConfiguration _config;

        private IWebDriver driver;
        private ConcurrentBag<ManagedWebDriver> _drivers;
        private DriverService _driverService;
        private DriverOptions _driverOptions;

        private Browser _browser;
        private bool _initialized;

        private IEnumerable<int> _pidsBefore;

        private string WebDriverExePath => _config.WebDriverExecutetableFilePath;
        private string ImagePath => _config.ImageDownloadPath;

        public WebDriverCollector(
            ILogger<WebDriverCollector> logger,
            IMessageQueue mq,
            SpiderClientConfiguration config)
        {
            _logger = logger;
            _mq = mq;
            _config = config;

            _pidsBefore = Process.GetProcessesByName("chromedriver").Concat(Process.GetProcessesByName("chrome")).Select(p => p.Id);
        }

        private async Task InitializeAsync()
        {
            if (!Directory.Exists(ImagePath))
            {
                Directory.CreateDirectory(ImagePath);
            }
            _drivers = new ConcurrentBag<ManagedWebDriver>();
            _browser = (Browser)Enum.Parse(typeof(Browser), _config.Browser ?? Browser.Chrome.ToString(), true);
            switch (_browser)
            {
                case Browser.Chrome:
                    {
                        _driverService = ChromeDriverService.CreateDefaultService(WebDriverExePath);
                        //disable command prompt output
                        _driverService.HideCommandPromptWindow = true;
                        _driverService.SuppressInitialDiagnosticInformation = true;
                        var chromeOptions = new ChromeOptions();
                        #region proxy setting
                        if (_config.IsUseProxy)
                        {
                            Proxy proxy = new Proxy();
                            proxy.Kind = ProxyKind.Manual;
                            ProxyProtocol protocol = (ProxyProtocol)Enum.Parse(typeof(ProxyProtocol), _config.ProxyProtocol, true);
                            switch (protocol)
                            {
                                case ProxyProtocol.Http:
                                    proxy.HttpProxy = _config.ProxyAddress;
                                    break;
                                case ProxyProtocol.SSL:
                                    proxy.SslProxy = _config.ProxyAddress;
                                    break;
                                case ProxyProtocol.Ftp:
                                    proxy.FtpProxy = _config.ProxyAddress;
                                    break;
                                case ProxyProtocol.Socks:
                                    proxy.SocksProxy = _config.ProxyAddress;
                                    break;
                            }
                            chromeOptions.Proxy = proxy;
                        }                        
                        #endregion
                        chromeOptions.AddArguments(new[] { "disable-infobars", "headless", "silent", "log-level=3", "no-sandbox", "disable-dev-shm-usage" });
                        _driverOptions = chromeOptions;
                        driver = new ChromeDriver((ChromeDriverService)_driverService, chromeOptions, TimeSpan.FromSeconds(60));
                        driver.Manage().Window.Size = new System.Drawing.Size(1296, 696);                                                
                        await Task.Factory.StartNew(() =>
                        {
                            var process = new Process();
                            
                            while (_drivers.Count < _config.WebDriverProcessCount)
                            {
                                var webdriver = new ChromeDriver((ChromeDriverService)_driverService, chromeOptions, TimeSpan.FromSeconds(_config.WebDriverTimeoutInSeconds));
                                _drivers.Add(new ManagedWebDriver(webdriver));
                                _logger.LogDebug("just added a new web driver, now drivers count {count}", _drivers.Count);
                            }
                        });
                        break;
                    }
                case Browser.Firefox:
                    {
                        FirefoxDriverService firefoxService = FirefoxDriverService.CreateDefaultService(WebDriverExePath);
                        firefoxService.HideCommandPromptWindow = true;
                        firefoxService.SuppressInitialDiagnosticInformation = true;
                        FirefoxOptions firefoxOptions = new FirefoxOptions();
                        firefoxOptions.AddArguments(new[] { "disable-infobars", "headless", "silent", "log-level=3", "no-sandbox", "disable-dev-shm-usage" });
                        driver = new FirefoxDriver(firefoxService, firefoxOptions, TimeSpan.FromSeconds(60));
                        driver.Manage().Window.Size = new System.Drawing.Size(1296, 696);
                        break;
                    }
                case Browser.IE:
                    {
                        InternetExplorerDriverService ieService = InternetExplorerDriverService.CreateDefaultService(WebDriverExePath);
                        ieService.HideCommandPromptWindow = true;
                        ieService.SuppressInitialDiagnosticInformation = true;
                        driver = new InternetExplorerDriver(ieService);
                        break;
                    }
                case Browser.Edge:
                    {
                        EdgeDriverService edgeService = EdgeDriverService.CreateDefaultService(WebDriverExePath);
                        edgeService.HideCommandPromptWindow = true;
                        edgeService.SuppressInitialDiagnosticInformation = true;
                        driver = new EdgeDriver(edgeService);
                        break;
                    }
                case Browser.Safari:
                    {
                        SafariDriverService safariService = SafariDriverService.CreateDefaultService(WebDriverExePath);
                        safariService.HideCommandPromptWindow = true;
                        safariService.SuppressInitialDiagnosticInformation = true;
                        driver = new SafariDriver(safariService);
                        break;
                    }
            }
            _initialized = true;
        }
        
        private Task<ManagedWebDriver> GetWebDriverInstance()
        {
            var driver = _drivers.FirstOrDefault(md => !md.Used);
            while (driver == null)
            {                
                _logger.LogInformation("wating one second to get web driver instance");
                Task.Delay(10000).Wait();
                driver = _drivers.FirstOrDefault(md => !md.Used);
            }

            driver.Used = true;
            return Task.FromResult(driver);            
        }

        public async Task CollectAsync(SpiderAtom atom)
        {
            if (!_initialized)
            {
                await InitializeAsync();
            }
            atom.Response = new HttpResponse { Success = true };
            ManagedWebDriver md = null;
            try
            {
                md = await GetWebDriverInstance(); ;
                var webDriver = md.WebDriver;
                webDriver.Navigate().GoToUrl(atom.Request.Url);
                webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                WebDriverWait wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(_config.WebDriverTimeoutInSeconds));            
                wait.IgnoreExceptionTypes(new Type[] { typeof(NoSuchElementException) });
                wait.Until((webdriver) =>
                {
                    var sel = atom.SpiderItem.GetTargetSelector().Result;
                    webdriver.FindElement(By.XPath(sel.Path));                    
                    return true;
                }); 
                atom.Response.Content = webDriver.PageSource;
                atom.Response.ContentType = atom.Request.ContentType;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "collect error in {collector}", nameof(WebDriverCollector));
                atom.Response.Success = false;
                atom.Response.ErrorMsg = e.Message;
                await _mq.PublishAsync(_config.TopicStatisticsFail, new Message
                {
                    MessageType = ErrorMessageType.CollectorError.ToString(),
                    Data = atom
                });
            }
            finally
            {
                md.Used = false;
            }
        }

        public void Dispose()
        {
            foreach (var driver in _drivers)
            {
                driver.WebDriver.Quit();
                driver.WebDriver.Dispose();
            }

            _driverService?.Dispose();
            var pidsAfter = Process.GetProcessesByName("chromedriver").Concat(Process.GetProcessesByName("chrome")).Select(p => p.Id);
            var webdriverPids = pidsAfter.Except(_pidsBefore);
            foreach (var pid in webdriverPids)
            {
                _logger.LogInformation("Killing pid: {0}", pid);
                Process.GetProcessById(pid).Kill();
            }

        }
    }

    public class ManagedWebDriver
    {
        public ManagedWebDriver(IWebDriver driver)
        {
            WebDriver = driver;
            Used = false;
        }

        public IWebDriver WebDriver { get; }

        public bool Used { get; set; }
    }
}
