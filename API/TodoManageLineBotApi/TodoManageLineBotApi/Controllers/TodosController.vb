Imports System.Web.Http

Public Class TodosController
    Inherits ApiController

    Private ReadOnly _resolveAzureBlobsRepository As IResolveAzureBlobsRepository
    Private ReadOnly _resolveTodoService As IResolveTodoService

    Public Sub New()
        _resolveAzureBlobsRepository = New ResolveAzureBlobsRepository()
        _resolveTodoService = New ResolveTodoService()
    End Sub

    ' GET api/todos
    Public Async Function GetTodos() As Threading.Tasks.Task(Of IEnumerable(Of Todos))
        Await _resolveAzureBlobsRepository.DownloadCsvAsync()
        Return _resolveTodoService.GetTodayTodos()
    End Function

    ' POST api/todos
    Public Async Function PostTodos(<FromBody()> todo As Todos) As Threading.Tasks.Task
        _resolveTodoService.UpdateTodos(todo)
        Await _resolveAzureBlobsRepository.UploadCsvAsync()
    End Function

    ' PUT api/todos
    Public Async Function PutTodos(<FromBody()> todo As Todos) As Threading.Tasks.Task
        _resolveTodoService.UpdateTodos(todo)
        Await _resolveAzureBlobsRepository.UploadCsvAsync()
    End Function

    <Route("api/isalive")>
    Public Function GetAlive() As String
        Return Date.Now.ToString()
    End Function
End Class
