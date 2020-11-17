# Using S3 with .NET Core on AWS

The used architecture:
https://github.com/ardalis/CleanArchitecture

## Creating Our Amazon S3 Client

Install `AWSSDK.S3` in the LifeBackup.Api and LifeBackup.Infrastructure projects.
Install `AWSSDK.Extensions.NETCore.Setup` in the LifeBackup.Api project.

In Startup.cs > ConfigureServices:

    services.AddAWSService<IAmazonS3>(_configuration.GetAWSOptions());

Allows AWS to get the information from `appsettings.json`.

    {
      "AWS": {
        "Profile": "lifebackup-profile",
        "Region": "us-west-2"
      }
    }


## Adding a Global Exception Handler:

In LifeBackup.Api:

    install-package Newtonsoft.Json

Startup.cs >> Configure:

    app.UseExceptionHandler(a => a.Run(async context =>
    {
      var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
      var exception = exceptionHandlerPathFeature.Error;
      var result = JsonConvert.SerializeObject(new {error = exception.Message});
      context.Response.ContentType = "application/json";
      await context.Response.WriteAsync(result);
    }));


## Installing the AWS Command Line Interface

https://docs.aws.amazon.com/cli/latest/userguide/install-windows.html

In a terminal the command `aws configure` should return:


    AWS Access Key ID [********************]: ...
    AWS Secret Access Key [********************]: ...
    Default region name [us-east-1]: us-west-2
    Default output format [json]:

Go to the file `C:\Users\{username}\.aws\credentials` and change to be equal:

    [lifebackup-profile]
    aws_access_key_id = ...
    aws_secret_access_key = ...
    


## Working with Buckets in S3 Using the AWS SDK

High Level API: 

- Provides a higher level of abstraction.

Low level API: 

- Requires you to write more code.
- Gives you greater control when dealing with objects


## Add Files to Amazon S3 Bucket

### Transfer Utility:

- High level utility: provides a high-level utility for managing transfers to and from Amazon S3.
- Multipart Uploads: makes extensive use of Amazon S3 multipart uploads.
- Multiple thread uploads: Will use multiple threads to upload multiple parts of a single upload at once.


# Creating a Test Framework for Amazon S3:
## Introduction

### LocalStack
LocalStack provider an easy-to-use test/mocking framework for developing applications hosted in AWS.

### Local vs. Real Amazon S3

- Allows integration tests to run faster
- Provides cost savings
- Could be out of date with the real Amazon S3


## Creating our Integration test project

Add a new project `Tests/LifeBackup.Integration.Tests`

In this project:

    install-package microsoft.aspnetcore.app -version 2.2.0
    install-package microsoft.aspnetcore.mvc.testing -version 2.2.0


https://xunit.net/docs/shared-context
To see more about the ICollectionFixture and TestContext class

## Docker and LocalStack Setup

In the project `Tests\LifeBackup.Integration.Tests`:

    install-package docker.dotnet -version 3.125.2



