# UI Metadata Framework app template

This project is a starter template for buildings apps with UI Metadata Framework. The core technologies used:

* [UI Metadata Framework](https://github.com/UNOPS/UiMetadataFramework) - the UIMF client used in this template is built on [Svelte](https://svelte.technology/)
* ASP.NET Core 3.1
* Entity Framework Core 3
* Microsoft SQL Server

## To setup the project

1. Publish *UiMFTemplate.Database* to an Sql Server instance.

## To run project

1. Inside *UiMFTemplate.Web/svelte-client* run `npm install`, then `npm run dev`.
2. Run *UiMFTemplate.DataSeed.App*. This is a console application which will add some sample data to the database.
3. Run *UiMFTemplate.Web*
4. You can now login with username `admin@example.com` and password `Password1`.

## How to rename project and all its files?

Use [rename-project.ps1](./rename-project.ps1) powershell script to rename all folders and files to match your app's name. The script will also replace text inside all files to use your app's name.
