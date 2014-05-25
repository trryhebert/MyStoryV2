USE [ShareUrStoryNew3]
GO
/****** Object:  StoredProcedure [dbo].[Admin_GetDeals]    Script Date: 2014-05-10 3:22:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Stories_Get]
	@StartResult int = 0,
	@EndResult int = 0,
	@Filter varchar(255) = '',
	@SearchTerm varchar(255) = '',
	@UserId int = 0
AS
BEGIN

	Declare @sql nVarchar(max)
	
	If Exists (Select * From tempdb..sysobjects Where id = object_id(N'[tempdb]..[#tmpStories]'))
		Drop Table [dbo].[#tmpStories]

	Create Table #tmpStories(DealId bigint null, ShortText varchar(Max) null, LongText varchar(Max) null, 
		TrackingUrl varchar(Max) null, CreativeUrl varchar(Max) null, StartDate datetime null, EndDate datetime null, 
		MasterCompanyName Varchar(max), AffiliateNetworkName Varchar(150) null, hasCreativeUrl Bit null)

	--Search Results
	Insert Into #tmpAdminDeals(DealId, ShortText, LongText, TrackingUrl, CreativeUrl, StartDate, EndDate,
				MasterCompanyName, AffiliateNetworkName, hasCreativeUrl)
	Select d.DealId, IsNull(d.ShortText, '') As ShortText, IsNull(d.LongText, '') As LongText, 
				IsNull(d.TrackingUrl, '') As TrackingUrl, IsNull(d.CreativeUrl, '') As CreativeUrl, 
				d.StartDate, d.EndDate, IsNull(c.Name, '') As MasterCompanyName, IsNull(r.Name, '') As AffiliateNetworkName,
				Case When IsNull(d.CreativeUrl, '') = '' Then 0 Else 1 End As hasCreativeUrl
	From Deals d 
	Left Join MasterCompanies c on d.MasterCompanyId = c.MasterCompanyId
	Left Join AffiliateNetworks r on d.AffiliateNetworkId = r.AffiliateNetworkId
	Where IsNull(d.EndDate, GetDate() + 1) >= GetDate()
	And d.DealID Not In(Select DealID From TopDeals)
	And (@MasterCompanyID = 0 Or d.MasterCompanyID = @MasterCompanyID)
	And (@StartDate Is Null Or CONVERT(VARCHAR(20), d.StartDate, 111) = CONVERT(VARCHAR(20), @StartDate, 111))
	
	--Check to make sure sorting columns exists in table, if not then reset sorting value
	if(@Sorting = '')
		Set @Sorting = 'DealID Desc'
	Declare @SortingCheck varchar(255)
	Set @SortingCheck = Replace(@Sorting, ' Desc', '')
	Set @SortingCheck = Replace(@SortingCheck, ' Asc', '')
	
	Declare @ColumnName varchar(255)
	Declare ColumnNames Cursor for
	Select item from fnSplit(@SortingCheck, ',')
	Open ColumnNames
	Fetch Next From ColumnNames into @ColumnName
	While @@Fetch_Status = 0
	Begin
		If Not Exists(Select * From TempDB.INFORMATION_SCHEMA.COLUMNS Where Table_Name Like '#tmpAdminDeals%' And Column_Name = @ColumnName)
			Set @Sorting = ''
		
		Fetch Next From ColumnNames into @ColumnName
	End
	Close ColumnNames
	Deallocate ColumnNames
	
	--Return Total Results Count
	Select Count(*) As TotalResults
	From #tmpAdminDeals
	
	--Return The Results	
	--Sorting and Paging 	
	If @EndResult = 0
		Set @EndResult = 999999999
		
	Set @sql = ' Select *
				From (Select  Row_Number() Over(Order By ' + @Sorting + ') As row, * 
					  From #tmpAdminDeals) As tbl
				Where row >= ' + CONVERT(varchar(9), @StartResult) + '
					  And row <=  ' + CONVERT(varchar(9), @EndResult)
	Exec (@sql) 

END