# Pattern
Projeto de criação de um sistema simples de conta Corrente para desevolvimento de padrões de projeto.

# Como Criar o projeto
Param(
	 [Parameter(Mandatory=$true)][String] $Name
)

function Add-DirApp {
    Write-Host "#################################################"
    Write-Host "#################################################"
    Write-Host "#####          CRIANDO DIRETORIOS          #####"
    Write-Host "#################################################"
    Write-Host "#################################################"
    Write-Host ""
    Write-Host ""
    Write-Host ""
    
    $currentPath = Get-Location

    if(Test-Path ("{0}\{1}" -f $currentPath, $AppName)){
        Write-Host ("O diretório {0} já existe!!!" -f $AppName)
        return 1
    }

    # Criar Pasta Base
    mkdir $AppName
}  


# Gerar Arquivo Docker
function Add-Dockerfile {
    
    [CmdletBinding()]
    param (
        [Parameter(Mandatory)]
        [string]$AppName
    )

    ('
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
ARG APP_VERSION=1.0.0.0
RUN dotnet nuget add source https://tfsprd01-rj01.modal.net.br/tfs/Projetos/Canais-Inovacao-Estrutural/_packaging/Estrutural/nuget/v3/index.json -u "nuget" -p "zciao7gcfkxpv6vq5nau7d4ym2pirx523hnve5pc64qixzkip6ca" --store-password-in-clear-text --valid-authentication-types "basic"
RUN dotnet nuget add source https://tfsprd01-rj01.modal.net.br/tfs/Projetos/White%20Label/_packaging/WhiteLabel/nuget/v3/index.json -u "nuget" -p "ikrectvtrfeqnvnnpk5dogcfqmp2rohcmogk5vrs5pisr72migwq" --store-password-in-clear-text --valid-authentication-types "basic"
COPY src/. .
RUN echo $APP_VERSION
RUN dotnet restore "{0}.Api/{0}.Api.csproj"
RUN dotnet publish "{0}.Api/{0}.Api.csproj" -c Release -r linux-x64 -o /app/publish -p:Version=$APP_VERSION


FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_ENVIRONMENT="Development"
ENV TZ="America/Sao_Paulo"
WORKDIR /app
COPY --from=build /app/publish .
COPY --from=busybox /bin/busybox /bin/busybox

ENTRYPOINT ["dotnet", "{0}.Api.dll"]
    ' -f $AppName) | Out-File "Dockerfile" -Encoding "utf8"
    
}

# Gerar Pasta Nuget
function Add-Nuget {
    mkdir .nuget

    # Gerar arquivo Nuget
    '<?xml version="1.0" encoding="utf-8"?>
    <configuration>
      <packageSources>
            <clear />
            <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />		    
            <add key="Estrutural" value="https://tfsprd01-rj01.modal.net.br/tfs/Projetos/Canais-Inovacao-Estrutural/_packaging/Estrutural/nuget/v3/index.json" />
            <add key="WhiteLabel" value="https://tfsprd01-rj01.modal.net.br/tfs/Projetos/White%20Label/_packaging/WhiteLabel/nuget/v3/index.json" />
      </packageSources>
      <activePackageSource>
        <add key="All" value="(Aggregate source)" />
      </activePackageSource>    
    </configuration>' | Out-File ".nuget/nuget.config" -Encoding "utf8"
}

function Add-DirBase {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory)]
        [string]$currentPath
    )

    # Gerar Pasta Src
    if(-Not (Test-Path ("{0}\src" -f $currentPath))){
        mkdir src
    }

    # Gerar Pasta Test
    if(-Not (Test-Path ("{0}\test" -f $currentPath))){
        mkdir test
    }
}

function Add-Project {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory)]
        [string]$AppName,

        [Parameter(Mandatory)]
        [string]$currentPath
    )

    Write-Host "#################################################"
    Write-Host "#################################################"
    Write-Host "#####          CRIANDO PROJETOS             #####"
    Write-Host "#################################################"
    Write-Host "#################################################"
    Write-Host ""
    Write-Host ""
    Write-Host ""

    #Adionar gitignore
    dotnet new gitignore
    (Get-Content (".gitignore")).replace('*.app', '') | Set-Content (".gitignore")

    # Define os nomes	
    $apiName = ("{0}.Api" -f $AppName)
    $applicationName = ("{0}.App" -f $AppName)
    #$domainName = ("{0}.Domain" -f $AppName)
    $infra = ("{0}.Infra" -f $AppName)
    $test = ("{0}.Test" -f $AppName)

    # Cria os projetos
    Set-Location src

    dotnet new webapi -lang "c#" -n $apiName --no-https
    dotnet new classlib -lang "c#" -n $applicationName
    #dotnet new classlib -lang "c#" -n $domainName
    dotnet new classlib -lang "c#" -n $infra

    Set-Location ..\test

    dotnet new xunit -lang "C#" -n $test


    Write-Host "#################################################"
    Write-Host "#################################################"
    Write-Host "#####        ADICIONANDO REFERENCIAS        #####"
    Write-Host "#################################################"
    Write-Host "#################################################"
    Write-Host ""
    Write-Host ""
    Write-Host ""

    # Define as referências
    Set-Location ..
    #dotnet add ("{0}\src\{1}\{1}.csproj" -f $currentPath, $applicationName) reference ("{0}\src\{1}\{1}.csproj" -f $currentPath, $domainName)
    #dotnet add ("{0}\src\{1}\{1}.csproj" -f $currentPath, $infra) reference ("{0}\src\{1}\{1}.csproj" -f $currentPath, $domainName)
    Add-Reference-InfraProject -currentPath $currentPath -infra $infra -applicationName $applicationName
    #dotnet add ("{0}\src\{1}\{1}.csproj" -f $currentPath, $infra) reference ("{0}\src\{1}\{1}.csproj" -f $currentPath, $applicationName)
    Add-Reference-ApiProject -currentPath $currentPath -apiName $apiName -infra $infra -applicationName $applicationName
    #dotnet add ("{0}\src\{1}\{1}.csproj" -f $currentPath, $apiName) reference ("{0}\src\{1}\{1}.csproj" -f $currentPath, $infra)
    #dotnet add ("{0}\src\{1}\{1}.csproj" -f $currentPath, $apiName) reference ("{0}\src\{1}\{1}.csproj" -f $currentPath, $applicationName)
    Add-Reference-TestProject -currentPath $currentPath -test $test -apiName $apiName
    #dotnet add ("{0}\test\{1}\{1}.csproj" -f $currentPath, $test) reference ("{0}\src\{1}\{1}.csproj" -f $currentPath, $apiName)

    #dotnet add ("{0}\src\{1}\{1}.csproj" -f $currentPath, $apiName) package MB.Core -v 6.0.4    
    Add-Reference-AppProject -currentPath $currentPath -applicationName $applicationName
    #dotnet add ("{0}\src\{1}\{1}.csproj" -f $currentPath, $applicationName) package Modal.BaaS.Proxy.Plugin.Core -v 6.0.15
    #dotnet add ("{0}\src\{1}\{1}.csproj" -f $currentPath, $applicationName) package MediatR -v 9.0.0
    #dotnet add ("{0}\src\{1}\{1}.csproj" -f $currentPath, $applicationName) package MediatR.Extensions.Microsoft.DependencyInjection -v 9.0.0
    #dotnet add ("{0}\src\{1}\{1}.csproj" -f $currentPath, $applicationName) package Swashbuckle.AspNetCore.Annotations -v 6.3.0


    Write-Host "#################################################"
    Write-Host "#################################################"
    Write-Host "#####           CRIANDO A SOLUTION          #####"
    Write-Host "#################################################"
    Write-Host "#################################################"
    Write-Host ""
    Write-Host ""
    Write-Host ""

    # Cria a Solution e adiciona os projetos
    dotnet new sln -n  ("{0}" -f $AppName)

    dotnet sln ("{0}\{1}.sln" -f $currentPath, $AppName) add ("{0}\src\{1}\{1}.csproj" -f $currentPath, $apiName)
    dotnet sln ("{0}\{1}.sln" -f $currentPath, $AppName) add ("{0}\src\{1}\{1}.csproj" -f $currentPath, $applicationName)
    #dotnet sln ("{0}\{1}.sln" -f $currentPath, $AppName) add ("{0}\src\{1}\{1}.csproj" -f $currentPath, $domainName)
    dotnet sln ("{0}\{1}.sln" -f $currentPath, $AppName) add ("{0}\src\{1}\{1}.csproj" -f $currentPath, $infra)
    dotnet sln ("{0}\{1}.sln" -f $currentPath, $AppName) add ("{0}\test\{1}\{1}.csproj" -f $currentPath, $test)
    #>


}
function Add-Reference-AppProject {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory)]
        [string]$currentPath,

        [Parameter(Mandatory)]
        [string]$applicationName
    )
    #Add Package
    dotnet add ("{0}\src\{1}\{1}.csproj" -f $currentPath, $applicationName) package Modal.BaaS.Proxy.Plugin.Core -v 6.0.15
    dotnet add ("{0}\src\{1}\{1}.csproj" -f $currentPath, $applicationName) package MediatR -v 9.0.0
    dotnet add ("{0}\src\{1}\{1}.csproj" -f $currentPath, $applicationName) package MediatR.Extensions.Microsoft.DependencyInjection -v 9.0.0
    dotnet add ("{0}\src\{1}\{1}.csproj" -f $currentPath, $applicationName) package Swashbuckle.AspNetCore.Annotations -v 6.3.0
}


function Add-Reference-ApiProject {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory)]
        [string]$currentPath,
        
        [Parameter(Mandatory)]
        [string]$apiName,
        
        [Parameter(Mandatory)]
        [string]$infra,

        [Parameter(Mandatory)]
        [string]$applicationName
    )
    #Add Reference
    dotnet add ("{0}\src\{1}\{1}.csproj" -f $currentPath, $apiName) reference ("{0}\src\{1}\{1}.csproj" -f $currentPath, $infra)
    dotnet add ("{0}\src\{1}\{1}.csproj" -f $currentPath, $apiName) reference ("{0}\src\{1}\{1}.csproj" -f $currentPath, $applicationName)
    #Add Package
    dotnet add ("{0}\src\{1}\{1}.csproj" -f $currentPath, $apiName) package MB.Core -v 6.0.4
}

function Add-Reference-TestProject {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory)]
        [string]$currentPath,
        
        [Parameter(Mandatory)]
        [string]$test,

        [Parameter(Mandatory)]
        [string]$apiName
    )
    #Add Reference
    dotnet add ("{0}\test\{1}\{1}.csproj" -f $currentPath, $test) reference ("{0}\src\{1}\{1}.csproj" -f $currentPath, $apiName)
}
function Add-Reference-InfraProject {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory)]
        [string]$currentPath,
        
        [Parameter(Mandatory)]
        [string]$infra,

        [Parameter(Mandatory)]
        [string]$applicationName
    )
    #Add Reference
    dotnet add ("{0}\src\{1}\{1}.csproj" -f $currentPath, $infra) reference ("{0}\src\{1}\{1}.csproj" -f $currentPath, $applicationName)

}

function Add-Program {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory)]
        [string]$AppName,

        [Parameter(Mandatory)]
        [string]$currentPath
    )

    $apiName = ("{0}.Api" -f $AppName)
    $programFile = ("{0}\src\{1}\Program.cs" -f $currentPath, $apiName)
    

    Remove-Item $programFile -Force
    #Remove-Item $startupFile -Force

    ('using System.Threading.Tasks;
    using MB.Core;

    namespace ApiNamespace
    {
        public class Program
        {
            public static async Task Main(string[] args)
            {
                await Microservice<Startup>.Run("ApiNamespace", args);
            }
        }
    }
    ' -replace 'ApiNamespace', $apiName) | Out-File $programFile -Encoding "utf8"
}

function Add-Startup {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory)]
        [string]$AppName,

        [Parameter(Mandatory)]
        [string]$currentPath
    )

    $apiName = ("{0}.Api" -f $AppName)
    $startupFile = ("{0}\src\{1}\Startup.cs" -f $currentPath, $apiName)

    ('
    using Modal.BaaS.Proxy.Plugin.Core.Extensions;
    using System;
    using MB.Core.Extensions;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    namespace ApiNamespace
    {
        public class Startup
        {
            private readonly IConfiguration _configuration;

            public Startup(IConfiguration configuration)
            {
                _configuration = configuration;
            }
            
            public void ConfigureServices(IServiceCollection services)
            {                        
                //services.AddCustomMediator();
                services.AddDefaultController();
                services.AddDefaultHealthCheck();
                services.AddHealthCheckPublisher(_configuration);
            }
            
            public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            {            
                app.MapAll(string.Empty, env);
                //app.AddCultureInfoDefault();
            }
        }
    }' -replace 'ApiNamespace', $apiName) | Out-File $startupFile -Encoding "utf8"
}


function Add-PropertyGroup {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory)]
        [string]$AppName,

        [Parameter(Mandatory)]
        [string]$currentPath
    )

    $apiName = ("{0}.Api" -f $AppName)

    (Get-Content ("{0}\src\{1}\{1}.csproj" -f $currentPath, $apiName)).replace('</PropertyGroup>', 
    '   <AppendTargetFrameworkToOutputPath>false</AppendTargetFrameworkToOutputPath>
        <ImplicitUsings>enable</ImplicitUsings>
        <GenerateDocumentationFile>true</GenerateDocumentationFile>
        <NoWarn>$(NoWarn);1591</NoWarn>
    </PropertyGroup>'
    ) | Set-Content ("{0}\src\{1}\{1}.csproj" -f $currentPath, $apiName)

}

function Add-MediatorExtensions{
    [CmdletBinding()]
    param (
        [Parameter(Mandatory)]
        [string]$AppName,

        [Parameter(Mandatory)]
        [string]$currentPath
    )
    
    # Gerar pasta Extensions em .App
    $extensionsFolder = ("{0}\src\{1}.App\Extensions" -f $currentPath, $AppName)

    if(-Not (Test-Path $extensionsFolder)){
        mkdir $extensionsFolder
    }

    # Gerar arquivo MediatorExtensions
    ('using MediatR;
    using Microsoft.Extensions.DependencyInjection;
    using System.Reflection;

    namespace ApplicationNamespace.App.Extensions
    {
        public static class MediatorExtensions
        {
            public static void AddCustomMediator(this IServiceCollection services)
            {
                services.AddMediatR(Assembly.GetExecutingAssembly());
            }
        }
    }' -replace 'ApplicationNamespace', $AppName) | Out-File ("{0}\MediatorExtensions.cs" -f $extensionsFolder) -Encoding "utf8"
}

function Build-Solution {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory)]
        [string]$AppName,

        [Parameter(Mandatory)]
        [string]$currentPath
    )

    Write-Host "#################################################"
    Write-Host "#################################################"
    Write-Host "#####              COMPILANDO              #####"
    Write-Host "#################################################"
    Write-Host "#################################################"
    Write-Host ""
    Write-Host ""
    Write-Host ""

    # Compila a solution
    dotnet build ("{0}\{1}.sln" -f $currentPath, $AppName)
}





$AppName = "Modal.BaaS.Proxy." + $Name

Add-DirApp
Set-Location $AppName
$currentPath = Get-Location
Add-Dockerfile -AppName $AppName
Add-Nuget
Add-DirBase -currentPath $currentPath
Add-Project -AppName $AppName -currentPath $currentPath
Add-Program -AppName $AppName -currentPath $currentPath
Add-Startup -AppName $AppName -currentPath $currentPath
Add-MediatorExtensions -AppName $AppName -currentPath $currentPath
Build-Solution -AppName $AppName -currentPath $currentPath


Write-Host " "
Write-Host " "
Write-Host " "
Write-Host " "





Write-Host "   _____             .___      .__    __________                 _________"
Write-Host "  /     \   ____   __| _/____  |  |   \______   \_____  _____   /   _____/"
Write-Host " /  \ /  \ /  _ \ / __ |\__  \ |  |    |    |  _/\__  \ \__  \  \_____  \ "
Write-Host "/    Y    (  <_> ) /_/ | / __ \|  |__  |    |   \ / __ \_/ __ \_/        \"
Write-Host "\____|__  /\____/\____ |(____  /____/  |______  /(____  (____  /_______  /"
Write-Host "        \/            \/     \/               \/      \/     \/        \/ "
Write-Host " "
Write-Host " "
Write-Host " "
Write-Host "Template criado com sucesso :) !!!"

Set-Location ..

Read-Host
