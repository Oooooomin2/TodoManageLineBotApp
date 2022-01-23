Imports System.Globalization
Imports System.IO
Imports CsvHelper
Imports CsvHelper.Configuration

Public Interface IResolveTodoService
    Function GetTodayTodos() As IEnumerable(Of Todos)
    Function GetUndoTodos() As IEnumerable(Of Todos)
    Function IsExistTodayTodo() As Boolean
    Sub UpdateTodos(todo As Todos)
    Function CreateSendData(message As String) As LineMessageApi
End Interface

Public Class ResolveTodoService
    Implements IResolveTodoService

    Private ReadOnly _tmpPath As String
    Private ReadOnly _userId As String
    Private ReadOnly _blobName As String

    Public Sub New()
        _tmpPath = ConfigurationManager.AppSettings("TmpPath")
        _userId = ConfigurationManager.AppSettings("UserId")
        _blobName = ConfigurationManager.AppSettings("BlobName")
    End Sub

    Public Sub UpdateTodos(todo As Todos) Implements IResolveTodoService.UpdateTodos
        Dim config = New CsvConfiguration(CultureInfo.InvariantCulture) With {
            .HasHeaderRecord = False
        }

        Using stream = File.Open($"{_tmpPath}/{_blobName}", FileMode.Append)
            Using writer = New StreamWriter(stream)
                Using csv = New CsvWriter(writer, config)
                    csv.WriteRecords(
                        New List(Of Todos) From
                        {
                            New Todos With
                            {
                                .Title = todo.Title,
                                .ImplementationDate = Date.Parse(todo.ImplementationDate),
                                .Status = todo.Status,
                                .CreatedDate = Date.Now
                            }
                        })
                End Using
            End Using
        End Using
    End Sub

    Public Function GetTodayTodos() As IEnumerable(Of Todos) Implements IResolveTodoService.GetTodayTodos
        '最終的に返却するListを生成
        Dim todayTodos As New List(Of Todos)()

        Using reader = New StreamReader($"{_tmpPath}/{_blobName}")
            Using csv = New CsvReader(reader, CultureInfo.InvariantCulture)
                Dim groupTodosByTitleAndDate = csv.GetRecords(Of Todos) _
                    .ToList() _
                    .Where(Function(o) Date.Parse(o.ImplementationDate).Date = Date.Now.Date) _
                    .GroupBy(Function(o) Tuple.Create(o.Title, o.ImplementationDate))

                For Each groupTodos In groupTodosByTitleAndDate
                    '未完了と完了のものがある場合は完了のtodoを返却する
                    If groupTodos.Count() > 1 Then
                        todayTodos.Add(groupTodos.Single(Function(o) o.Status = "完"))
                    Else
                        todayTodos.Add(groupTodos.First())
                    End If
                Next

                Return todayTodos
            End Using
        End Using
    End Function

    Public Function GetUndoTodos() As IEnumerable(Of Todos) Implements IResolveTodoService.GetUndoTodos
        '最終的に返却するListを生成
        Dim undoTodos As New List(Of Todos)()

        Using reader = New StreamReader($"{_tmpPath}/{_blobName}")
            Using csv = New CsvReader(reader, CultureInfo.InvariantCulture)
                Dim groupTodosByTitleAndDate = csv.GetRecords(Of Todos) _
                    .ToList() _
                    .Where(Function(o) Date.Parse(o.ImplementationDate).Date = Date.Now.Date) _
                    .GroupBy(Function(o) Tuple.Create(o.Title, o.ImplementationDate))

                For Each groupTodos In groupTodosByTitleAndDate
                    '完了となっていないもののみ通知対象とする
                    If groupTodos.Count() = 1 Then
                        undoTodos.Add(groupTodos.First())
                    End If
                Next

                Return undoTodos
            End Using
        End Using
    End Function

    Public Function CreateSendData(message As String) As LineMessageApi Implements IResolveTodoService.CreateSendData
        Return New LineMessageApi With
        {
            .UserId = _userId,
            .Messages =
            {
                New Messages With
                {
                    .Type = "text",
                    .Text = message
                }
            }
        }
    End Function

    Public Function IsExistTodayTodo() As Boolean Implements IResolveTodoService.IsExistTodayTodo
        Using reader = New StreamReader($"{_tmpPath}/{_blobName}")
            Using csv = New CsvReader(reader, CultureInfo.InvariantCulture)
                Return csv.GetRecords(Of Todos) _
                    .ToList() _
                    .Any(Function(o) o.ImplementationDate.Date = Date.Now.Date)
            End Using
        End Using
    End Function
End Class
