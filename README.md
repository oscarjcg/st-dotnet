dotnet ef dbcontext info --project st-dotnet
dotnet ef migrations add create_categories --project st-dotnet
dotnet ef database update --project st-dotnet
dotnet ef migrations remove --project st-dotnet

