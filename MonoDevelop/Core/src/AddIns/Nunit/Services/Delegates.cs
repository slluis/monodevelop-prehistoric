using System;
using NUnit.Core;

namespace MonoDevelop.Services.Nunit
{
	delegate void FixtureAddedEventHandler (object sender, FixtureAddedEventArgs args);
    delegate void FixtureLoadErrorHandler (object sender, FixtureLoadErrorEventArgs args);
    delegate void TestStartHandler (TestCase test);
    delegate void TestFinishHandler (TestCaseResult result);
    delegate void SuiteStartHandler (TestSuite test);
    delegate void SuiteFinishHandler (TestSuiteResult result);
    delegate void TestCaseResultHandler (TestResult result);
}
