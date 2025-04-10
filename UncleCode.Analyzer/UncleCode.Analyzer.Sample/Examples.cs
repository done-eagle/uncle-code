using System.Collections;
using System;
using Microsoft.CSharp;
using Microsoft.SqlServer;
using Serilog.Context;
using Newtonsoft.Json;

namespace UncleCode.Analyzer.Sample;

public class Examples
{
    public class MyCompanyClass
    {
    }

    public void ToStars()
    {
        var spaceship = new Spaceship();
        spaceship.SetSpeed(300000000);
        spaceship.SetSpeed(42);
    }
}