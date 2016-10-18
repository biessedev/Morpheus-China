﻿Option Explicit On

Imports MySql.Data.MySqlClient
Imports System.IO
Imports System.Net
Imports System.Data.SqlClient
Imports System.Globalization
Imports System.Net.Dns
Imports System.Xml


Module PublicFunction
    Dim AdapterCh As New MySqlDataAdapter("SELECT * FROM mant", MySqlconnection)
    Dim dsCh As New DataSet
    Dim tblCh As New DataTable
    Public MySqlconnection As MySqlConnection
    Public MySqlconnection_3DEQTable As MySqlConnection
    Public MySqlconnectionGru As New MySqlConnection
    Public SQLconnectionOrcad As New SqlConnection
    Public MySQLConnectionString As String
    Public ConnectionString As String
    Public GroupList As String, OpenIssue As String, ProdOpenIssue As String
    Public StrComboBoxGroup As String
    Public Declare Function ShellExecute Lib "shell32.dll" Alias "ShellExecuteA" (ByVal hWnd As Long, ByVal lpOperation As String, ByVal lpFile As String, ByVal lpParameters As String, ByVal lpDirectory As String, ByVal nShowCmd As Long) As Long
    Public DBName As String
    Public tblError As DataTable
    Public DsError As New DataSet
    Public CultureInfo_ja_JP As New System.Globalization.CultureInfo("ja-JP", False)
    Public strFtpServerAdd As String
    Public strFtpServerUser As String
    Public strFtpServerPsw As String
    Dim ConnectionStringOrcad As String
    Public Const CstrRevNoteCreation As String = "Creation file"
    Public strEqHour As String
    Public strEqCost As String
    Public strEqDEs As String
    Public MemProcess As String
    Public NPIDocName As String

    Public NoEcrWaitingCheck As Integer
    Public NoEcrWaitingApprove As Integer
    Public NoEcrWaitingSign As Integer

    Public R_Pending, E_Pending, L_Pending, P_Pending, Q_Pending, U_Pending, B_Pending, N_Pending As eCR_Pending



    Structure eCR_Pending
        Dim Checked_Pending As Integer
        Dim Approved_Pending As Integer
        Dim Sign_Pending As Integer
        Dim Pending_Check_Name As String
        Dim Pending_Approve_Name As String
        Dim Pending_Sign_Name As String

    End Structure

    Structure credential
        Dim strUserName As String
        Dim strPassword As String
        Dim strHost As String
        Dim strDatabase As String
        Dim strSign As String
        Dim intId As Integer
    End Structure

    Public CreAccount As New credential

    Structure FileRecord
        Dim Header As String
        Dim Rev As Integer
        Dim FileName As String
        Dim Path As String
        Dim RevNote As String
        Dim Sign As String
        Dim Extension As String
    End Structure

    Public CreFile As New FileRecord

    Sub OpenConnectionMySql(ByVal strHost As String, ByVal strDatabase As String, ByVal strUserName As String, ByVal strPassword As String)

        Try
            ConnectionString = "host=" & strHost & ";" & "username=" & strUserName & ";" & "password=" & strPassword & ";" & "database=" & strDatabase & ";Connect Timeout=120;allow zero datetime=true; "
            MySqlconnection = New MySqlConnection(ConnectionString)
            If MySqlconnection.State = ConnectionState.Open Then
                MySqlconnection.Close()
            End If
            MySqlconnection.Open()
        Catch ae As MySqlException
            MessageBox.Show(ae.Message.ToString())
        End Try


    End Sub

    Sub CloseConnectionMySql()
        Try
            If MySqlconnection.State = ConnectionState.Open Then
                MySqlconnection.Close()
            End If
        Catch ae As MySqlException
            MessageBox.Show(ae.Message.ToString())
        End Try
    End Sub

    Sub OpenConnectionMySql_3DEQTable(ByVal strHost As String)

        Try
            ConnectionString = "host=" & strHost & ";" & "username=" & "3deqtable" & ";" & "password=" & "3deqtable" & ";" & "database=" & "srvdoc" & ";Connect Timeout=120;allow zero datetime=true;"
            MySqlconnection_3DEQTable = New MySqlConnection(ConnectionString)
            If MySqlconnection_3DEQTable.State = ConnectionState.Open Then
                MySqlconnection_3DEQTable.Close()
            End If
            MySqlconnection_3DEQTable.Open()
        Catch ae As MySqlException
            MessageBox.Show(ae.Message.ToString())
        End Try
    End Sub

    Sub OpenConnectionMySqlGru(ByVal strHost As String, ByVal strDatabase As String, ByVal strUserName As String, ByVal strPassword As String)
        Dim ConnectionStringGru As String
        Try
            ConnectionStringGru = "Allow Zero Datetime=true; host=" & strHost & "; username=" & strUserName & ";" & "password=" & strPassword & ";" & "database=" & strDatabase & ";Connect Timeout=120;"
            MySqlconnectionGru = New MySqlConnection(ConnectionStringGru)
            If MySqlconnectionGru.State = ConnectionState.Open Then
                MySqlconnectionGru.Close()
            End If
            MySqlconnectionGru.Open()
        Catch ae As MySqlException
            MessageBox.Show(ae.Message.ToString())
        End Try
    End Sub

    Sub OpenConnectionMySqlOrcad(ByVal strHost As String, ByVal strDatabase As String, ByVal strUserName As String, ByVal strPassword As String)
        Dim ConnectionStringOrcad As String
        Try
            ConnectionStringOrcad = "server=" & strHost & ";user id=" & strUserName & ";" & "pwd=" & strPassword & ";" & "database=" & strDatabase & ";Connect Timeout=120;"
            SQLconnectionOrcad = New SqlConnection(ConnectionStringOrcad)
            If SQLconnectionOrcad.State = ConnectionState.Open Then
                SQLconnectionOrcad.Close()
            End If
            SQLconnectionOrcad.Open()
        Catch ae As MySqlException
            MessageBox.Show(ae.Message.ToString())
        End Try
    End Sub

    Sub CloseConnectionMySqlGru()

        Try
            If MySqlconnectionGru.State = ConnectionState.Open Then
                MySqlconnectionGru.Close()
            End If
        Catch ae As MySqlException
            MessageBox.Show(ae.Message.ToString())
        End Try
    End Sub

    Sub CloseConnectionSqlOrcad()

        Try
            If SQLconnectionOrcad.State = ConnectionState.Open Then
                SQLconnectionOrcad.Close()
            End If
        Catch ae As MySqlException
            MessageBox.Show(ae.Message.ToString())
        End Try
    End Sub

    Function cap7(ByVal s As String) As String
        cap7 = UCase(Mid(s, 1, 7)) & (Mid(s, 8))
    End Function

    Function controlRight(ByVal header As String) As Integer
        Dim intpos As Integer
        intpos = InStr(CreAccount.strSign, header, CompareMethod.Text)
        If intpos Then
            controlRight = Val(Mid(CreAccount.strSign, intpos + 1, 1))
        Else
            controlRight = 0
        End If
    End Function

    Declare Function GetTempPath Lib "kernel32.dll" Alias "GetTempPathA" (ByVal nBufferLength As Long, ByVal lpBuffer As String) As Long
    Public Const MAX_BUFFER_LENGTH = 256


    Public Function getTempPathName() As String
        Dim strBufferString As String
        Dim lngResult As Long
        strBufferString = StrDup(MAX_BUFFER_LENGTH, "X")
        lngResult = GetTempPath(MAX_BUFFER_LENGTH, strBufferString)
        getTempPathName = Mid(strBufferString, 1, lngResult)
    End Function

    Public Function ToDateTime(ByVal _
          dataGG_MM_AAAA As String) As DateTime

        Dim myCultureInfo As CultureInfo = CultureInfo.CurrentCulture
        dataGG_MM_AAAA = Replace(dataGG_MM_AAAA, "-", "/")
        Dim formato As String = "MM/dd/yyyy"
        Return _
          Date.ParseExact(dataGG_MM_AAAA, _
             formato, myCultureInfo)
    End Function


    Function s(ByVal sval As Object) As String
        If IsDBNull(sval) Then
            s = ""
        Else
            s = sval
        End If
    End Function


    ' Esempi

    'Sub pippo()
    '    Dim fwr As FtpWebRequest = FtpWebRequest.Create("ftp://localhost/pippo.zip")
    '    fwr.Method = WebRequestMethods.Ftp.ListDirectory


    '    fwr.Credentials = New NetworkCredential("test", "test")

    '    Dim sr As New StreamReader(fwr.GetResponse().GetResponseStream())

    '    Dim str As String = sr.ReadLine()

    '    While Not str Is Nothing

    '        Console.WriteLine(str)

    '        str = sr.ReadLine()

    '    End While

    '    sr.Close()

    '    sr = Nothing

    '    fwr = Nothing
    'End Sub

    Function StrSettingRead(ByVal ComCode As String) As String
        Dim rsResult As DataRow()
        rsResult = tblError.Select("code='" & ComCode & "'")
        If rsResult.Length = 0 Then
            ComCode = "0051"
            rsResult = tblError.Select("code='" & ComCode & "'")
            StrSettingRead = rsResult(0).Item("en").ToString & " " & ComCode
        Else
            StrSettingRead = rsResult(0).Item("en").ToString
        End If

    End Function


    Function intranetHeader(ByVal h As String) As Boolean

        If h = "65R_PRO_GPN" Then intranetHeader = True
        If h = "65R_PRO_GFX" Then intranetHeader = True
        If h = "65R_PRO_NFB" Then intranetHeader = True
        If h = "65R_PRO_NFP" Then intranetHeader = True
        If h = "65R_PRO_PCB" Then intranetHeader = True
        If h = "65R_PRO_PST" Then intranetHeader = True
        If h = "65R_PRO_SPG" Then intranetHeader = True
        If h = "65R_PRO_TDS" Then intranetHeader = True

    End Function

    Sub WriteCheckTable(ByVal des As String)

        AdapterCh.SelectCommand = New MySqlCommand("SELECT * FROM mant ", MySqlconnection)
        AdapterCh.Fill(dsCh, "mant")
        tblCh = dsCh.Tables("mant")

        Dim returnValue As DataRow(), sql As String, cmd As MySqlCommand
        returnValue = tblCh.Select("des = '" & des & "'")
        If returnValue.Length >= 1 Then
            Try
                sql = "UPDATE `" & DBName & "`.`mant` SET `des` = '" & des & _
                "', `data` = '" & "[" & date_to_string(Now) & "]" & _
                "'WHERE `mant`.`id` = " & returnValue(0).Item("id").ToString & " ;"

                cmd = New MySqlCommand(sql, MySqlconnection)
                cmd.ExecuteNonQuery()
            Catch ex As Exception

            End Try
        Else
            Try
                sql = "INSERT INTO `" & DBName & "`.`mant`(`des`, `data`) VALUES ('" & _
                des & "', '[" & date_to_string(Now) & "]'" & ");"

                cmd = New MySqlCommand(sql, MySqlconnection)
                cmd.ExecuteNonQuery()

                AdapterCh.SelectCommand = New MySqlCommand("SELECT * FROM mant ", MySqlconnection)
                dsCh.Clear()
                tblCh.Clear()
                AdapterCh.Fill(dsCh, "mant")
                tblCh = dsCh.Tables("mant")

            Catch ex As Exception

            End Try
        End If
    End Sub

    Function user() As String
        user = ""
        If controlRight("E") = 3 Then user = "E"
        If controlRight("L") = 3 Then user = "L"
        If controlRight("P") = 3 Then user = "P"
        If controlRight("Q") = 3 Then user = "Q"
        If controlRight("R") = 3 Then user = "R"
        If controlRight("U") = 3 Then user = "U"
        If controlRight("A") = 3 Then user = "A"
        If controlRight("N") = 3 Then user = "N"
        If controlRight("C") = 3 Then user = "C"
        If controlRight("F") = 3 Then user = "F"
        If controlRight("B") = 3 Then user = "B"
    End Function

    Sub WriteFile(ByVal a As String, ByVal append As Boolean)

        ' Create an instance of StreamWriter to write text to a file.
        Using sw = New StreamWriter(Path.GetTempPath & "SrvQueryLog.txt", append)
            ' Add some text to the file.
            sw.WriteLine(a)
            sw.Close()
        End Using

        'Using sw As StreamReader = New StreamReader(System.IO.Path.GetTempPath & "SrvQueryLog.txt")
        '    ' Add some text to the file.
        '    a = sw.ReadLine
        '    sw.Close()
        'End Using


    End Sub

    Sub WriteTxtFile(ByVal file As String, ByVal text As String, ByVal append As Boolean)

        ' Create an instance of StreamWriter to write text to a file.
        Using sw = New StreamWriter(file, append)
            ' Add some text to the file.
            sw.WriteLine(text)
            sw.Close()
        End Using

    End Sub

    Function ExportListview2Excel(ByVal lstview As ListView) As Boolean
        Dim SaveFileDialog1 As New SaveFileDialog
        Dim csvFileContents As New System.Text.StringBuilder
        Dim CurrLine As String = String.Empty
        'Write out the column names as headers for the csv file.
        For columnIndex = 0 To lstview.Columns.Count - 1
            CurrLine &= (String.Format("{0};", lstview.Columns(columnIndex).Text))
        Next
        'Remove trailing comma
        csvFileContents.AppendLine(CurrLine.Substring(0, CurrLine.Length - 1))
        CurrLine = String.Empty
        'Write out the data.
        For Each item As ListViewItem In lstview.Items
            For Each subItem As ListViewItem.ListViewSubItem In item.SubItems
                CurrLine &= (String.Format("{0};", subItem.Text))
            Next
            'Remove trailing comma
            csvFileContents.AppendLine(CurrLine.Substring(0, CurrLine.Length - 1))
            CurrLine = String.Empty
        Next
        'Create the file.
        SaveFileDialog1.FileName = "ProductList.csv"
        SaveFileDialog1.ShowDialog()
        Try
            Dim Sys As New StreamWriter(SaveFileDialog1.FileName)
            Sys.WriteLine(csvFileContents.ToString)
            Sys.Flush()
            Sys.Dispose()
        Catch ex As Exception

        End Try


    End Function



    Function date_to_string(ByVal Indate As Date) As String
        date_to_string = Indate.Year & "/" & Mid("0" & Indate.Month, Len(Trim(Str(Indate.Month))), 2) & "/" & Mid("0" & Indate.Day, Len(Trim(Str(Indate.Day))), 2)

    End Function

    Function string_to_date(ByVal Indate As String) As Date
        If Len(Indate) >= 8 Then string_to_date = DateTime.Parse(Indate, CultureInfo_ja_JP.DateTimeFormat)
    End Function



    'Write and get the time of server.
    Function MySqlServerTimeString() As String
        Dim cmd As New MySqlCommand(), sql As String

        Try
            sql = "UPDATE `" & DBName & "`.`parameterset` SET `value` =  NOW() +0 where name = 'sessionTime'"
            cmd = New MySqlCommand(sql, MySqlconnection)
            cmd.ExecuteNonQuery()
        Catch ex As Exception
            MsgBox("Time Write error!")
        End Try
        Dim Adapter As New MySqlDataAdapter("SELECT * FROM parameterset where name = 'sessionTime'", MySqlconnection)
        Dim tbl As DataTable
        Dim Ds As New DataSet
        Adapter.Fill(Ds, "parameterset")
        tbl = Ds.Tables("parameterset")
        MySqlServerTimeString = ParameterTable("sessionTime")

    End Function

    ' convert in dataTime the server string datatime
    Function MySqlServerDataTime() As DateTime
        MySqlServerDataTime = DateTime.ParseExact(MySqlServerTimeString, "yyyyMMddHHmmss", CultureInfo.InvariantCulture)
    End Function

    ' SETTA RESETTA SESSIONE
    Function session(ByVal bomName As String, ByVal id As Long, ByVal setT_clearF As Boolean) As String
        session = "ID_0"
        If id <> 0 Then
            Dim SessionTime As String, SessionUser As String
            Dim cmd As New MySqlCommand(), sql As String
            Dim Adapter As New MySqlDataAdapter("SELECT * FROM " & bomName & " where id = " & id, MySqlconnection)
            Dim tbl As DataTable
            Dim Ds As New DataSet
            Adapter.Fill(Ds, bomName)
            tbl = Ds.Tables(bomName)
            Try
                SessionTime = tbl.Rows(0).Item("SessionTime").ToString()
                SessionUser = tbl.Rows(0).Item("SessionUser").ToString()
            Catch ex As Exception

            End Try

            sql = ""
            If SessionTime = "" Then
                If setT_clearF = True Then
                    sql = "UPDATE `" & DBName & "`.`" & bomName & "` SET `SessionTime` =  now() +0,`SessionUser` = '" & CreAccount.strUserName & "' where id= " & id
                    session = "SET"
                Else
                    session = "RESET"
                End If
            Else
                If SessionUser = CreAccount.strUserName Then
                    If setT_clearF = True Then
                        sql = "UPDATE `" & DBName & "`.`" & bomName & "` SET `SessionTime` =  now() +0,`SessionUser` = '" & CreAccount.strUserName & "' where id= " & id
                        session = "SET"
                    Else
                        sql = "UPDATE `" & DBName & "`.`" & bomName & "` SET `SessionTime` = '',`SessionUser` = '' where id= " & id
                        session = "RESET"
                    End If
                Else
                    If DeltaSessionTime(bomName, id) > 30 Then
                        If setT_clearF = True Then
                            sql = "UPDATE `" & DBName & "`.`" & bomName & "` SET `SessionTime` =  now() +0,`SessionUser` = '" & CreAccount.strUserName & "' where id= " & id
                            session = "SET"
                        Else
                            sql = "UPDATE `" & DBName & "`.`" & bomName & "` SET `SessionTime` = '',`SessionUser` = '' where id= " & id
                            session = "RESET"
                        End If
                    Else
                        session = "USED " & SessionUser & " TIME: " & SessionTime
                    End If
                End If
            End If

            If sql <> "" Then
                Try
                    cmd = New MySqlCommand(sql, MySqlconnection)
                    cmd.ExecuteNonQuery()
                Catch ex As Exception
                    MsgBox("Set session error!")
                End Try
            End If

        End If
    End Function


    Function DeltaSessionTime(ByVal TableName As String, ByVal id As Long) As Integer
        Dim SessionTime As String, SessionUser As String
        Dim cmd As New MySqlCommand()
        Dim Adapter As New MySqlDataAdapter("SELECT * FROM " & TableName & " where id = " & id, MySqlconnection)
        Dim tbl As DataTable
        Dim Ds As New DataSet
        Adapter.Fill(Ds, TableName)
        tbl = Ds.Tables(TableName)
        SessionTime = tbl.Rows(0).Item("SessionTime").ToString()
        SessionUser = tbl.Rows(0).Item("SessionUser").ToString()
        If SessionTime <> "" Then
            DeltaSessionTime = -DateDiff("n", DateTime.ParseExact(MySqlServerTimeString, "yyyyMMddHHmmss", CultureInfo.InvariantCulture), DateTime.ParseExact(SessionTime, "yyyyMMddHHmmss", CultureInfo.InvariantCulture))
        End If
    End Function


    'Write and get the time of server.
    Function ParameterTable(ByVal param As String) As String
        Try
        Dim Adapter As New MySqlDataAdapter("SELECT * FROM parameterset", MySqlconnection)
        Dim tbl As DataTable
        Dim Ds As New DataSet, resultRow As DataRow()
        Adapter.Fill(Ds, "parameterset")
        tbl = Ds.Tables("parameterset")
        resultRow = tbl.Select("name = '" & param & "'")
        If resultRow.Length > 0 Then
            ParameterTable = resultRow(0).Item("value").ToString()
        End If
            Adapter.Dispose()
            Ds.Dispose()
        Catch ex As Exception
            MsgBox("Error: " & ex.Message)
        End Try

    End Function

    Function ParameterTableWrite(ByVal param As String, ByVal value As String) As String

        Dim cmd As New MySqlCommand(), sql As String
        ParameterTableWrite = "KO"
        Try
            sql = "UPDATE `" & DBName & "`.`parameterset` SET `value` ='" & value & "' where name = '" & param & "'"
            cmd = New MySqlCommand(sql, MySqlconnection)
            cmd.ExecuteNonQuery()
            ParameterTableWrite = "OK"
        Catch ex As Exception
            MsgBox("Parametric Write error!   " & ex.Message)
        End Try

    End Function


    Public Class TVTFXException
        Inherits Exception
        Public Sub New(ByVal msg As String)
            MyBase.New(msg)
        End Sub
        Public Sub New(ByVal msg As String, ByVal ex As Exception)
            MyBase.New(msg, ex)
        End Sub
    End Class


    Public Class TreeViewToFromXml
        Private TVTFX_Tree As TreeView
        Private TVTFX_SavePath As String
        Private TVTFX_XmlDoc As New XmlDocument()

        Public Sub New(Optional ByVal path As String = Nothing, Optional ByRef tree As TreeView = Nothing)
            TVTFX_Tree = tree
            TVTFX_SavePath = path
        End Sub

        ''' <summary>
        ''' Sets the given TreeView
        ''' </summary>
        ''' <param name="tree"></param>
        ''' <remarks></remarks>
        Public Sub SetTreeView(ByRef tree As TreeView)
            If Not tree Is Nothing Then
                TVTFX_Tree = tree
            Else
                Throw New TVTFXException("TreeView cannot be Nothing")
            End If
        End Sub



        Public Sub CloseConnectionSqlOrcad()

            Try
                If SQLconnectionOrcad.State = ConnectionState.Closed Then
                    SQLconnectionOrcad.Open()
                End If
            Catch ex As Exception
                MessageBox.Show(ex.ToString())
            End Try
        End Sub

#Region "Export"
        ''' <summary>
        ''' Exports the Treeview as Xml to the given File
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ExportToFile(Optional ByVal path As String = Nothing) As Boolean

            If Not path Is Nothing Then
                TVTFX_SavePath = path
            End If

            If Not TVTFX_Tree Is Nothing And Not TVTFX_SavePath Is Nothing Then
                GenerateXml()
                Try
                    TVTFX_XmlDoc.Save(TVTFX_SavePath)
                Catch ex As Exception
                    Throw New TVTFXException("Error while saving File!", ex)
                End Try

            Else
                Throw New TVTFXException("Missing TreeView or SavePath!")
            End If

            Return True
        End Function

        ''' <summary>
        ''' Exports the Treeview to Xml
        ''' </summary>
        ''' <returns>Xml-Code as String</returns>
        ''' <remarks></remarks>
        Public Function ExportToString() As String
            If Not TVTFX_Tree Is Nothing Then
                GenerateXml()
                Return TVTFX_XmlDoc.OuterXml.ToString
            Else
                Throw New TVTFXException("Missing TreeView!")
            End If
        End Function

        ''' <summary>
        ''' Exports the Treeview to xml As XmlDocument
        ''' </summary>
        ''' <returns>XmlDocument</returns>
        ''' <remarks></remarks>
        Public Function ExportToXmlDocument() As XmlDocument
            If Not TVTFX_Tree Is Nothing Then
                GenerateXml()
                Return TVTFX_XmlDoc
            Else
                Throw New TVTFXException("Missing TreeView!")
            End If
        End Function

        ''' <summary>
        ''' Generates the valid XML
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub GenerateXml()
            Dim ienum As IEnumerator
            TVTFX_XmlDoc.LoadXml("<?xml version=""1.0"" " & "encoding=""ISO-8859-1""?><TreeView />")

            ienum = TVTFX_Tree.Nodes.GetEnumerator
            While ienum.MoveNext
                XmlAddNode(DirectCast(ienum.Current, TreeNode))
            End While
        End Sub
        ''' <summary>
        ''' Recursive Function for adding all Nodes to XmlDocument
        ''' </summary>
        ''' <param name="ActualTreeNode"></param>
        ''' <param name="ActualNode"></param>
        ''' <remarks></remarks>
        Private Sub XmlAddNode(ByVal ActualTreeNode As TreeNode, Optional ByVal ActualNode As XmlNode = Nothing)
            Dim ienum As IEnumerator
            Dim xmlKD As XmlElement

            If ActualNode Is Nothing Then
                xmlKD = TVTFX_XmlDoc.CreateElement(XmlConvert.EncodeName(ActualTreeNode.Text.Replace(":", ";")))
                TVTFX_XmlDoc.DocumentElement.AppendChild(xmlKD)
            Else
                xmlKD = TVTFX_XmlDoc.CreateElement(XmlConvert.EncodeName(ActualTreeNode.Text.Replace(":", ";")))
                ActualNode.AppendChild(xmlKD)
            End If
            'If (ActualTreeNode.Text.Chars(1).ToString = ":") Then
            'xmlKD.OuterXml = ActualTreeNode.Text.Substring(0, 2).ToString & xmlKD.LocalName
            'End If
            ienum = ActualTreeNode.Nodes.GetEnumerator
            While ienum.MoveNext
                XmlAddNode(DirectCast(ienum.Current, TreeNode), xmlKD)
            End While
        End Sub

#End Region

#Region "Import"

        ''' <summary>
        ''' Imports the TreeView using XML in given File
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns>True on success</returns>
        ''' <remarks></remarks>
        Public Function ImportFromFile(Optional ByVal path As String = Nothing) As Boolean
            If Not path Is Nothing Then
                TVTFX_SavePath = path
            End If
            Try
                TVTFX_XmlDoc.Load(TVTFX_SavePath)
            Catch ex As Exception
                Throw New TVTFXException("Error while loading File", ex)
            End Try
            GenerateTree()
            Return True

        End Function

        ''' <summary>
        ''' Imports the TreeView using given XML-Code
        ''' </summary>
        ''' <param name="xml">Valid Xml</param>
        ''' <returns>True on success</returns>
        ''' <remarks></remarks>
        Public Function Import(ByVal xml As String) As Boolean
            TVTFX_XmlDoc.LoadXml(xml)
            GenerateTree()
            Return True
        End Function

        ''' <summary>
        ''' Imports the TreeView using given XmlDocument
        ''' </summary>
        ''' <param name="xml">An XmlDocument</param>
        ''' <returns>True on success</returns>
        ''' <remarks></remarks>
        Public Function Import(ByVal xml As XmlDocument) As Boolean
            TVTFX_XmlDoc = xml
            GenerateTree()
            Return True
        End Function


        ''' <summary>
        ''' Generates the TreeView using the given XML
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub GenerateTree()
            TVTFX_Tree.Nodes.Clear()
            TVTFX_Tree.Nodes.Add(New TreeNode(TVTFX_XmlDoc.DocumentElement.Name))

            Dim StartNode As New TreeNode()
            StartNode = TVTFX_Tree.Nodes(0)

            TreeviewAddNode(TVTFX_XmlDoc.DocumentElement, StartNode)
            'frmTree.ExpandAll()
        End Sub

        ''' <summary>
        ''' Recursive Function for adding all Nodes to the TreeView
        ''' </summary>
        ''' <param name="TXmlNode"></param>
        ''' <param name="TreeViewNode"></param>
        ''' <remarks></remarks>
        Private Sub TreeviewAddNode(ByRef TXmlNode As XmlNode, ByRef TreeViewNode As TreeNode)
            Dim xml_SingleNode As XmlNode
            Dim xml_NodeList As XmlNodeList
            Dim trn_Node As TreeNode


            If TXmlNode.HasChildNodes() Then
                xml_NodeList = TXmlNode.ChildNodes

                For I = 0 To xml_NodeList.Count - 1
                    xml_SingleNode = TXmlNode.ChildNodes(I)

                    TreeViewNode.Nodes.Add(New TreeNode(XmlConvert.DecodeName(xml_SingleNode.Name).Replace(":", ":")))
                    trn_Node = TreeViewNode.Nodes.Item(I)
                    TreeviewAddNode(xml_SingleNode, trn_Node)
                Next

            Else
                TreeViewNode.Text = (XmlConvert.DecodeName(TXmlNode.Name).Replace(":", ":"))
            End If

        End Sub

        Public Sub CollectProcess()
            MemProcess = ""
            For Each prog As Process In Process.GetProcesses
                MemProcess = MemProcess & ";" & prog.Id

            Next
        End Sub

        Public Sub KillLastExcel()
            MemProcess = ""
            For Each prog As Process In Process.GetProcesses
                If prog.ProcessName = "EXCEL" And InStr(MemProcess, prog.Id) <= 0 Then
                    prog.Kill()
                End If
            Next
        End Sub

#End Region
    End Class
End Module
