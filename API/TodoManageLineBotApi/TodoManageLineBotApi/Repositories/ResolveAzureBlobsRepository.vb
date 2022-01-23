Imports System.IO
Imports System.Threading.Tasks
Imports Azure.Storage.Blobs

Public Interface IResolveAzureBlobsRepository
    Function DownloadCsvAsync() As Task
    Function UploadCsvAsync() As Task
End Interface


Public Class ResolveAzureBlobsRepository
    Implements IResolveAzureBlobsRepository

    Private ReadOnly _containerName As String
    Private ReadOnly _blobName As String
    Private ReadOnly _tmpPath As String
    Private ReadOnly _connectionString As String

    Public Sub New()
        _containerName = ConfigurationManager.AppSettings("ContainerName")
        _blobName = ConfigurationManager.AppSettings("BlobName")
        _tmpPath = ConfigurationManager.AppSettings("TmpPath")
        _connectionString = ConfigurationManager.AppSettings("ConnectionString")
    End Sub

    Public Async Function DownloadCsvAsync() As Task Implements IResolveAzureBlobsRepository.DownloadCsvAsync
        Dim container As New BlobContainerClient(_connectionString, _containerName)
        Dim blob As BlobClient = container.GetBlobClient(_blobName)

        If Not Directory.Exists(_tmpPath) Then
            Directory.CreateDirectory(_tmpPath)
        End If

        Await blob.DownloadToAsync($"{_tmpPath}/{_blobName}")
    End Function

    Public Async Function UploadCsvAsync() As Task Implements IResolveAzureBlobsRepository.UploadCsvAsync
        Dim container As New BlobContainerClient(_connectionString, _containerName)
        Dim blob As BlobClient = container.GetBlobClient(_blobName)
        Await blob.UploadAsync($"{_tmpPath}/{_blobName}", True)
    End Function
End Class
