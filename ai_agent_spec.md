cd /home/ameen/Desktop/BookN/bookN/bookn-powerful/backend && dotnet ef migrations add AmenityIconsAndCategories --project YemenBooking.Infrastructure --startup-project YemenBooking.Api


dotnet ef database update -p YemenBooking.Infrastructure -s YemenBooking.Api
