#if UNITY_2019_2_OR_NEWER

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.TestTools.TestRunner.Api;
using HardCodeLab.RockTomate.Core.Data;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;

[StepDescription("Run Unit Tests", "Runs unit tests using Unity Test Runner")]
public class RunUnitTestsStep : Step, IErrorCallbacks
{
    private bool _testsRunning;
    private bool _hasErrors;

    [InputField]
    public TestMode TestMode = (TestMode)(~0);

    [InputField(tooltip: "Test categories that will run. Leave empty to run all.")]
    public string[] Categories;

    [InputField(tooltip: "Test Assemblies that will run. Leave empty to run all.")]
    public string[] AssemblyNames;

    [InputField(tooltip: "Test that will run. Leave empty to run all.")]
    public string[] TestNames;

    [OutputField(tooltip: "Whether this test run has at least one failed test.")]
    public bool HasFailedTests = false;

    [OutputField(tooltip: "Executed test results. Only contains leaf tests (the ones with no tests nested within them).")]
    public List<ITestResultAdaptor> TestResults = new List<ITestResultAdaptor>();

    protected override IEnumerator OnExecute(JobContext context)
    {
        var testRunnerApi = ScriptableObject.CreateInstance<TestRunnerApi>();
        var filter = new Filter
        {
            testMode = TestMode,
            categoryNames = Categories,
            assemblyNames = AssemblyNames,
            testNames = TestNames,
        };

        testRunnerApi.RegisterCallbacks(this);
        testRunnerApi.Execute(new ExecutionSettings(filter));
        _testsRunning = true;

        while (_testsRunning)
            yield return null;

        testRunnerApi.UnregisterCallbacks(this);

        IsSuccess = !_hasErrors;
    }

    protected override void OnReset()
    {
        TestResults = new List<ITestResultAdaptor>();
        _testsRunning = false;
        _hasErrors = false;
    }

    public void RunStarted(ITestAdaptor testsToRun)
    {
        _testsRunning = true;
    }

    public void RunFinished(ITestResultAdaptor result)
    {
        _testsRunning = false;
    }

    public void TestStarted(ITestAdaptor test)
    {
    }

    public void TestFinished(ITestResultAdaptor result)
    {
        if (result.HasChildren)
            return;

        if (result.TestStatus == TestStatus.Failed)
            HasFailedTests = true;

        TestResults.Add(result);
    }

    public void OnError(string message)
    {
        _hasErrors = true;
    }
}

#endif