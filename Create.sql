USE [master]
GO

/****** Object:  Database [Cache]    Script Date: 9/7/2015 10:08:50 AM ******/
CREATE DATABASE [Cache]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Cache', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.MSSQLSERVER\MSSQL\DATA\Cache.mdf' , SIZE = 6144KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB ), 
 FILEGROUP [Cache_mod] CONTAINS MEMORY_OPTIMIZED_DATA  DEFAULT
( NAME = N'Cache_mod', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.MSSQLSERVER\MSSQL\DATA\Cache_mod' , MAXSIZE = UNLIMITED)
 LOG ON 
( NAME = N'Cache_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.MSSQLSERVER\MSSQL\DATA\Cache_log.ldf' , SIZE = 76736KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO

ALTER DATABASE [Cache] SET COMPATIBILITY_LEVEL = 120
GO


ALTER DATABASE [Cache] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [Cache] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [Cache] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [Cache] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [Cache] SET ARITHABORT OFF 
GO

ALTER DATABASE [Cache] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [Cache] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [Cache] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [Cache] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [Cache] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [Cache] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [Cache] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [Cache] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [Cache] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [Cache] SET  DISABLE_BROKER 
GO

ALTER DATABASE [Cache] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [Cache] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [Cache] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [Cache] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [Cache] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [Cache] SET READ_COMMITTED_SNAPSHOT OFF 
GO

ALTER DATABASE [Cache] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [Cache] SET RECOVERY SIMPLE 
GO

ALTER DATABASE [Cache] SET  MULTI_USER 
GO

ALTER DATABASE [Cache] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [Cache] SET DB_CHAINING OFF 
GO

ALTER DATABASE [Cache] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO

ALTER DATABASE [Cache] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO

ALTER DATABASE [Cache] SET DELAYED_DURABILITY = DISABLED 
GO

ALTER DATABASE [Cache] SET  READ_WRITE 
GO


USE [Cache]
GO

/****** Object:  Table [dbo].[ObjectCache]    Script Date: 9/7/2015 10:10:37 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ObjectCache]
(
	[ObjectKey] [uniqueidentifier] NOT NULL,
	[Expires] [datetime2](7) NOT NULL,
	[ObjectBody] [nvarchar](3950) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,

CONSTRAINT [imPK_ObjectCache_ObjectKey] PRIMARY KEY NONCLUSTERED HASH 
(
	[ObjectKey]
)WITH ( BUCKET_COUNT = 16384),
INDEX [IX_CreateDate] NONCLUSTERED 
(
	[Expires] ASC
)
)WITH ( MEMORY_OPTIMIZED = ON , DURABILITY = SCHEMA_ONLY )

GO


USE [Cache]
GO

/****** Object:  Table [dbo].[ObjectCache2]    Script Date: 9/7/2015 10:11:07 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ObjectCache2](
	[ObjectKey] [uniqueidentifier] NOT NULL,
	[Expires] [datetime2](7) NOT NULL,
	[ObjectBody] [nvarchar](3950) NOT NULL,
 CONSTRAINT [imPK_ObjectCache2_ObjectKey] PRIMARY KEY CLUSTERED 
(
	[ObjectKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO




USE [Cache]
GO

/****** Object:  StoredProcedure [dbo].[RetrieveCacheItem]    Script Date: 9/7/2015 10:11:28 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




create procedure [dbo].[RetrieveCacheItem] @uid uniqueidentifier
WITH
    NATIVE_COMPILATION, 
    SCHEMABINDING, 
    EXECUTE AS OWNER
AS
BEGIN ATOMIC
   WITH (TRANSACTION ISOLATION LEVEL=SNAPSHOT, LANGUAGE='us_english')
   
   select ObjectBody from dbo.ObjectCache where ObjectKey = @uid;
END


GO



USE [Cache]
GO

/****** Object:  StoredProcedure [dbo].[SaveCacheItem]    Script Date: 9/7/2015 10:11:42 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



create procedure [dbo].[SaveCacheItem] @uid uniqueidentifier, @body nvarchar(3950), @expiration datetime2
WITH
    NATIVE_COMPILATION, 
    SCHEMABINDING, 
    EXECUTE AS OWNER
AS
BEGIN ATOMIC
   WITH (TRANSACTION ISOLATION LEVEL=SNAPSHOT, LANGUAGE='us_english')
   
   delete [dbo].[ObjectCache] where ObjectKey = @uid;
   insert into [dbo].[ObjectCache] values (@uid, @expiration, SUBSTRING(@body,0,3950));
END

GO


USE [Cache]
GO

/****** Object:  StoredProcedure [dbo].[RetrieveCacheItem2]    Script Date: 9/7/2015 10:11:55 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


create procedure [dbo].[RetrieveCacheItem2] @uid uniqueidentifier
AS
BEGIN    
   select ObjectBody from dbo.ObjectCache2 where ObjectKey = @uid;
END


GO



USE [Cache]
GO

/****** Object:  StoredProcedure [dbo].[SaveCacheItem2]    Script Date: 9/7/2015 10:12:10 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


create procedure [dbo].[SaveCacheItem2] @uid uniqueidentifier, @body nvarchar(3950), @expiration datetime2
AS
BEGIN    
   delete [dbo].[ObjectCache2] where ObjectKey = @uid;
   insert into [dbo].[ObjectCache2] values (@uid, @expiration, SUBSTRING(@body,0,3950));
END



GO


