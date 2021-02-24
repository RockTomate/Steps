#if UNITY_2019_2_OR_NEWER

using System.Collections;
using UnityEngine;
using UnityEditor.TestTools.TestRunner.Api;
using HardCodeLab.RockTomate.Core.Data;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;

[StepDescription("Run Unit Tests", "Runs unit tests using Unity Test Runner")]
public class RunUnitTestsStep : Step
{
    [InputField]
    public TestMode TestMode = (TestMode)(~0);

    [InputField(tooltip: "Categories that will run. Leave empty to run all categories.")]
    public string[] Categories;

    protected override IEnumerator OnExecute(JobContext context)
    {
        var testCallbacks = new UnitTestsCallbacks();
        var testRunnerApi = ScriptableObject.CreateInstance<TestRunnerApi>();
        var filter = new Filter
        {
            testMode = TestMode,
            categoryNames = Categories,
        };

        testRunnerApi.RegisterCallbacks(testCallbacks);
        testRunnerApi.Execute(new ExecutionSettings(filter));

        while (testCallbacks.TestsRunning)
            yield return null;

        testRunnerApi.UnregisterCallbacks(testCallbacks);

        IsSuccess = !testCallbacks.HasErrors;
    }

    private class UnitTestsCallbacks : IErrorCallbacks
    {
        public bool TestsRunning = true;
        public bool HasErrors;

        public void RunStarted(ITestAdaptor testsToRun)
        {
            TestsRunning = true;
        }

        public void RunFinished(ITestResultAdaptor result)
        {
            TestsRunning = false;
        }

        public void TestStarted(ITestAdaptor test)
        {
        }

        public void TestFinished(ITestResultAdaptor result)
        {
        }

        public void OnError(string message)
        {
            HasErrors = true;
        }
    }
}

#endif