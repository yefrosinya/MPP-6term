using Tracer.Core;
using Tracer.Core.Models;
using Xunit;

namespace Tracer.Core.Tests;

public class TracerTests
{
    [Fact]
    public void StartTrace_ShouldRecordMethod()
    {
        var tracer = new Tracer();
        tracer.StartTrace();
        Thread.Sleep(10);
        tracer.StopTrace();

        var result = tracer.GetTraceResult();
        
        Assert.Single(result.Threads);
        Assert.Single(result.Threads[0].Methods);
        Assert.Equal("StartTrace_ShouldRecordMethod", result.Threads[0].Methods[0].MethodName);
        Assert.Equal("TracerTests", result.Threads[0].Methods[0].ClassName);
        Assert.True(result.Threads[0].Methods[0].TimeMs >= 10);
    }

    [Fact]
    public void StopTrace_ShouldStopTimer()
    {
        var tracer = new Tracer();
        tracer.StartTrace();
        Thread.Sleep(50);
        tracer.StopTrace();

        var result = tracer.GetTraceResult();
        
        Assert.True(result.Threads[0].Methods[0].TimeMs >= 50);
    }

    [Fact]
    public void GetTraceResult_ShouldReturnReadOnlyResult()
    {
        var tracer = new Tracer();
        tracer.StartTrace();
        tracer.StopTrace();

        var result = tracer.GetTraceResult();
        
        Assert.NotNull(result);
        Assert.NotNull(result.Threads);
        Assert.IsAssignableFrom<IReadOnlyList<ThreadResult>>(result.Threads);
    }

    [Fact]
    public void NestedMethods_ShouldBeRecordedCorrectly()
    {
        var tracer = new Tracer();
        
        tracer.StartTrace();
        Thread.Sleep(10);
        InnerMethod(tracer);
        tracer.StopTrace();

        var result = tracer.GetTraceResult();
        
        Assert.Single(result.Threads);
        var rootMethod = result.Threads[0].Methods[0];
        Assert.Single(rootMethod.Methods);
        Assert.Equal("InnerMethod", rootMethod.Methods[0].MethodName);
        Assert.True(rootMethod.Methods[0].TimeMs >= 20);
    }

    private void InnerMethod(ITracer tracer)
    {
        tracer.StartTrace();
        Thread.Sleep(20);
        tracer.StopTrace();
    }

    [Fact]
    public void MultipleRootMethods_ShouldBeRecorded()
    {
        var tracer = new Tracer();
        
        tracer.StartTrace();
        Thread.Sleep(10);
        tracer.StopTrace();
        
        tracer.StartTrace();
        Thread.Sleep(15);
        tracer.StopTrace();

        var result = tracer.GetTraceResult();
        
        Assert.Single(result.Threads);
        Assert.Equal(2, result.Threads[0].Methods.Count);
    }

    [Fact]
    public void MultipleThreads_ShouldBeRecordedSeparately()
    {
        var tracer = new Tracer();
        var thread1 = new Thread(() =>
        {
            tracer.StartTrace();
            Thread.Sleep(30);
            tracer.StopTrace();
        });
        
        var thread2 = new Thread(() =>
        {
            tracer.StartTrace();
            Thread.Sleep(40);
            tracer.StopTrace();
        });

        thread1.Start();
        thread2.Start();
        thread1.Join();
        thread2.Join();

        var result = tracer.GetTraceResult();
        
        Assert.Equal(2, result.Threads.Count);
    }

    [Fact]
    public void TotalTime_ShouldBeSumOfRootMethods()
    {
        var tracer = new Tracer();
        
        tracer.StartTrace();
        Thread.Sleep(25);
        tracer.StopTrace();
        
        tracer.StartTrace();
        Thread.Sleep(35);
        tracer.StopTrace();

        var result = tracer.GetTraceResult();
        
        var totalTime = result.Threads[0].TotalTimeMs;
        var sumOfMethods = result.Threads[0].Methods.Sum(m => m.TimeMs);
        
        Assert.True(totalTime >= 60);
        Assert.Equal(sumOfMethods, totalTime);
    }

    [Fact]
    public void MethodInfo_ShouldContainCorrectData()
    {
        var tracer = new Tracer();
        tracer.StartTrace();
        Thread.Sleep(5);
        tracer.StopTrace();

        var result = tracer.GetTraceResult();
        var method = result.Threads[0].Methods[0];
        
        Assert.NotNull(method.MethodName);
        Assert.NotNull(method.ClassName);
        Assert.True(method.TimeMs >= 5);
        Assert.NotNull(method.Methods);
        Assert.IsAssignableFrom<IReadOnlyList<MethodResult>>(method.Methods);
    }

    [Fact]
    public void DeepNesting_ShouldWorkCorrectly()
    {
        var tracer = new Tracer();
        
        tracer.StartTrace();
        Level1(tracer);
        tracer.StopTrace();

        var result = tracer.GetTraceResult();
        
        var level1 = result.Threads[0].Methods[0].Methods[0];
        var level2 = level1.Methods[0];
        var level3 = level2.Methods[0];
        
        Assert.Equal("Level1", level1.MethodName);
        Assert.Equal("Level2", level2.MethodName);
        Assert.Equal("Level3", level3.MethodName);
    }

    private void Level1(ITracer tracer)
    {
        tracer.StartTrace();
        Level2(tracer);
        tracer.StopTrace();
    }

    private void Level2(ITracer tracer)
    {
        tracer.StartTrace();
        Level3(tracer);
        tracer.StopTrace();
    }

    private void Level3(ITracer tracer)
    {
        tracer.StartTrace();
        Thread.Sleep(5);
        tracer.StopTrace();
    }

    [Fact]
    public void StopTrace_WithoutStartTrace_ShouldNotCrash()
    {
        var tracer = new Tracer();
        
        tracer.StopTrace();
        
        var result = tracer.GetTraceResult();
        
        Assert.Empty(result.Threads);
    }

    [Fact]
    public void MultipleStartTrace_WithoutStopTrace_ShouldHandleCorrectly()
    {
        var tracer = new Tracer();
        
        tracer.StartTrace();
        tracer.StartTrace();
        tracer.StopTrace();
        tracer.StopTrace();

        var result = tracer.GetTraceResult();
        
        Assert.Single(result.Threads);
        Assert.Single(result.Threads[0].Methods);
        Assert.Single(result.Threads[0].Methods[0].Methods);
    }
}
