﻿
Option Explicit On
Option Compare Text
Imports MySql.Data.MySqlClient
Imports System.IO
Imports System.Data.SqlClient
Imports System
Imports Microsoft.VisualBasic
Imports System.Runtime.InteropServices
Imports System.Globalization


Public Class FormCommit
    Dim AdapterCommitList As New MySqlDataAdapter("SELECT * FROM Commit_List", MySqlconnection)
    Dim tblCommitList As DataTable
    Dim DsCommitList As New DataSet
    Dim AdapterCommit As New MySqlDataAdapter("SELECT * FROM Commit", MySqlconnection)
    Dim tblCommit As DataTable
    Dim DsCommit As New DataSet


    Private Sub Commit_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Dim d As Date
        AdapterCommitList.Fill(DsCommitList, "Commit_List")
        tblCommitList = DsCommitList.Tables("Commit_List")

        AdapterCommit.Fill(DsCommit, "Commit")
        tblCommit = DsCommit.Tables("Commit")

        FillcomboCommit()
        UpdateTreecommit()

        DateTimePicker1.Value = Today
        DateTimePickerEnd.Text = ""
        d = string_to_date(DateTimePicker1.Text)
        LabelDay.Text = d.ToString("dddd")
        FillcomboUser()
        If controlRight("R") >= 3 Then
            ButtonRemoveCommit.Enabled = True
            ButtonNewCommit.Enabled = True
            ButtonReset.Enabled = True
            DateTimePickerEnd.Enabled = True
            DateTimePickerStart.Enabled = True
            ComboBoxUser.Enabled = True
        End If
        ComboBoxUser.Text = ComboBoxUser.Text
        TreeViewBomList.Focus()
        UpdateTreecommit()
        TextBoxMontly.Text = MonthHour(string_to_date(DateTimePicker1.Text))
        TextBoxOpen.Text = ""
        TextBoxClosed.Text = ""
    End Sub


    Private Sub DateTimePicker1_Validated(ByVal sender As Object, ByVal e As EventArgs) Handles DateTimePicker1.Validated
        ' aggiorna lista
        UpdateTreecommit()
        Dim d As Date
        TextBoxMontly.Text = MonthHour(string_to_date(DateTimePicker1.Text))
        TextBoxNote.Text = ""
        d = string_to_date(DateTimePicker1.Text)
        LabelDay.Text = d.ToString("dddd")
    End Sub


    Private Sub ButtonNewBom_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ButtonNewBom.Click
        Dim sql As String
        Dim cmd As New MySqlCommand()
        If TextBoxOpen.Text <> "" Then
            If DateDiff("d", string_to_date(DateTimePicker1.Text), string_to_date(TextBoxOpen.Text)) <= 0 Then
                If TextBoxClosed.Text = "" Then
                    If ComboBoxCommit.Text <> "" And IsNumeric(TextBoxHour.Text) And ((Val(TextBoxHour.Text) + Val(TextBoxDay.Text)) <= 24) Then
                        Try
                            sql = "INSERT INTO `" & DBName & "`.`commit` (`hour` ,`commit` ,`note` ,`date` ,`name`) VALUES ('" & _
                            TextBoxHour.Text & "', '" & _
                            ComboBoxCommit.Text & "', '" & _
                            TextBoxNote.Text & "', '" & _
                            date_to_string(DateTimePicker1.Text) & "', '" & _
                            ComboBoxUser.Text & "');"

                            cmd = New MySqlCommand(sql, MySqlconnection)
                            cmd.ExecuteNonQuery()
                        Catch ex As Exception
                            MsgBox("Insert Error !!")
                        End Try
                        UpdateTreecommit()
                    Else
                        MsgBox("Please check input data!")
                    End If
                Else
                    MsgBox("Commit closed!")
                End If
            Else
                MsgBox("Want set a job with open date before open date of commit!")
            End If
        Else
            MsgBox("Please select commit!")
        End If
        UpdateTreecommit()
        TextBoxMontly.Text = MonthHour(string_to_date(DateTimePicker1.Text))
    End Sub

    Private Sub ButtonBomRemove_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ButtonBomRemove.Click
        Dim cmd As New MySqlCommand()
        Dim sql As String
        If Not IsNothing(TreeViewBomList.SelectedNode) Then
            If controlRight("R") >= 2 And user(Trim(Mid(TreeViewBomList.SelectedNode.Text, 1, InStr(TreeViewBomList.SelectedNode.Text, "-") - 1))) = ComboBoxUser.Text Then

                If MsgBox("Want you delete this Record?", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
                    Try
                        sql = "DELETE FROM `" & DBName & "`.`commit` WHERE `commit`.`id` = '" & Trim(Mid(TreeViewBomList.SelectedNode.Text, 1, InStr(TreeViewBomList.SelectedNode.Text, "-") - 1)) & "'"
                        cmd = New MySqlCommand(sql, MySqlconnection)
                        cmd.ExecuteNonQuery()
                    Catch ex As Exception
                        MsgBox("Mysql delete error ")
                    End Try
                Else
                    MsgBox("please fill all field before update")
                End If
            Else
                MsgBox("no enough right")
            End If
        Else
            MsgBox("please select a item")
        End If
        UpdateTreecommit()
        TextBoxMontly.Text = MonthHour(string_to_date(DateTimePicker1.Text))
        TextBoxNote.Text = ""
    End Sub

    Private Sub TreeViewBomList_AfterSelect(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles TreeViewBomList.AfterSelect
        Dim id As Integer
        id = Val(Trim(Mid(TreeViewBomList.SelectedNode.Text, 1, InStr(TreeViewBomList.SelectedNode.Text, "-") - 1)))
        Dim rowShow As DataRow()
        DsCommit.Clear()
        tblCommit.Clear()
        AdapterCommit.Fill(DsCommit, "Commit")
        tblCommit = DsCommit.Tables("Commit")
        rowShow = tblCommit.Select("id=" & id, "name")
        If rowShow.Length > 0 Then
            TextBoxNote.Text = rowShow(0).Item("note").ToString()
        End If
        If CheckBoxMonthView.Checked = True Then DateTimePicker1.Text = Mid(TreeViewBomList.SelectedNode.Text, 1, 10)
    End Sub

    Private Sub ComboBoxCommit_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ComboBoxCommit.SelectedIndexChanged
        Dim rowResults As DataRow()
        DsCommitList.Clear()
        tblCommitList.Clear()
        AdapterCommitList.Fill(DsCommitList, "Commit_List")
        tblCommitList = DsCommitList.Tables("Commit_List")
        rowResults = tblCommitList.Select("name = '" & ComboBoxCommit.Text & "'", "name")
        If rowResults.Length > 0 Then
            TextBoxDescription.Text = rowResults(0).Item("description").ToString
            TextBoxOpen.Text = rowResults(0).Item("open").ToString
            TextBoxClosed.Text = rowResults(0).Item("closed").ToString
        Else
            TextBoxDescription.Text = ""
        End If
        If ComboBoxCommit.Text = "UN_COMMIT" Then TextBoxDescription.Text = "For all work without a commit. Pleae fill the Note"
        If ComboBoxCommit.Text = "HOLIDAY" Then TextBoxDescription.Text = "For all no work time"
        If ComboBoxCommit.Text = "UN_COMMIT" Or ComboBoxCommit.Text = "HOLIDAY" Then
            TextBoxOpen.Text = "2000/01/01"
            TextBoxClosed.Text = ""
        End If
    End Sub

    Private Sub CheckBoxUser_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CheckBoxUser.CheckedChanged
        Try
            DateTimePicker1.Text = date_to_string(Today)
            TextBoxMontly.Text = MonthHour(string_to_date(DateTimePicker1.Text))
            CheckBoxMonthView.Checked = False
            UpdateTreecommit()
        Catch ex As Exception
        End Try
    End Sub

    Private Sub DateTimePickerEnd_ValueChanged(ByVal sender As Object, ByVal e As EventArgs) Handles DateTimePickerEnd.ValueChanged
        Dim sql As String
        Dim cmd As New MySqlCommand()
        TextBoxClosed.Text = DateTimePickerEnd.Text
        Try
            sql = "UPDATE `" & DBName & "`.`commit_list` SET `closed` = '" & DateTimePickerEnd.Text & "' WHERE `commit_list`.`name` = '" & ComboBoxCommit.Text & "' ;"
            cmd = New MySqlCommand(sql, MySqlconnection)
            cmd.ExecuteNonQuery()
        Catch ex As Exception
            MsgBox("sql Error !!")
        End Try
    End Sub

    Private Sub DateTimePickerStart_ValueChanged(ByVal sender As Object, ByVal e As EventArgs) Handles DateTimePickerStart.ValueChanged
        'Dim sql As String
        'Dim cmd As New MySqlCommand()
        TextBoxOpen.Text = DateTimePickerStart.Text
        'Try
        '    sql = "UPDATE `" & DBName & "`.`commit_list` SET `open` = '" & DateTimePickerStart.Text & "' WHERE `commit_list`.`name` = '" & ComboBoxCommit.Text & "' ;"
        '    cmd = New MySqlCommand(sql, MySqlconnection)
        '    cmd.ExecuteNonQuery()
        'Catch ex As Exception
        '    MsgBox("sql Error !!")
        'End Try
    End Sub

    ' show the montth viewer.
    Private Sub CheckBoxMonthView_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CheckBoxMonthView.CheckedChanged, CheckBoxCommit.TextChanged, TextBoxWindows.TextChanged
        Dim d As Date
        If DateTimePicker1.Text <> "" Then
            d = string_to_date(DateTimePicker1.Text)
            If CheckBoxMonthView.Checked = True Then
                ButtonBomRemove.Enabled = False
                ButtonNewBom.Enabled = False
                TreeViewBomList.Nodes.Clear()
                For i = -Val(TextBoxWindows.Text) To Val(TextBoxWindows.Text)
                    Dim rootNode As TreeNode
                    rootNode = New TreeNode(date_to_string(DateAdd("d", i, d)) & " - Hour/s : ..." & IIf(dayHour(date_to_string(DateAdd("d", i, d))) > 9, "", ".") & dayHour(date_to_string(DateAdd("d", i, d))))
                    If dayHour(date_to_string(DateAdd("d", i, d))) < 8 Then
                        rootNode.ForeColor = Color.LightGreen
                    ElseIf dayCommit(date_to_string(DateAdd("d", i, d))) = "MIXED" Then
                        rootNode.ForeColor = Color.Green
                    ElseIf dayCommit(date_to_string(DateAdd("d", i, d))) = "UN_COMMIT" Then
                        rootNode.ForeColor = Color.Blue
                    ElseIf dayCommit(date_to_string(DateAdd("d", i, d))) = "HOLIDAY" Then
                        rootNode.ForeColor = Color.Black
                    Else
                        rootNode.ForeColor = Color.Green
                    End If

                    If DateAdd("d", i, d).DayOfWeek = DayOfWeek.Saturday Or DateAdd("d", i, d).DayOfWeek = DayOfWeek.Sunday Then
                        rootNode.BackColor = Color.LightPink
                        rootNode.ForeColor = Color.Red
                    End If
                    TreeViewBomList.Nodes.Add(rootNode)
                Next
            Else
                ButtonBomRemove.Enabled = True
                ButtonNewBom.Enabled = True
                UpdateTreecommit()
            End If
        End If
    End Sub

    Private Sub CheckBoxCommit_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CheckBoxCommit.CheckedChanged
        Try
            DateTimePicker1.Text = date_to_string(Today)
            TextBoxMontly.Text = MonthHour(string_to_date(DateTimePicker1.Text))
            CheckBoxMonthView.Checked = False
            UpdateTreecommit()
        Catch ex As Exception
        End Try
    End Sub

    Private Sub ButtonRemoveCommit_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ButtonRemoveCommit.Click

        Dim cmd As New MySqlCommand()
        Dim sql As String

        If Not CommitmentJob(ComboBoxCommit.Text) Then
            If ComboBoxCommit.Text <> "" Then
                Try
                    sql = "DELETE FROM `" & DBName & "`.`commit_list` WHERE `commit_list`.`name` = '" & ComboBoxCommit.Text & "'"
                    cmd = New MySqlCommand(sql, MySqlconnection)
                    cmd.ExecuteNonQuery()
                Catch ex As Exception
                    MsgBox("Mysql delete error " & ex.Message)
                End Try
            Else
                MsgBox("please fill commit name")
            End If
        Else
            MsgBox("Need to delete all job for all user for delete this commit!")
        End If

        UpdateTreecommit()
        TextBoxMontly.Text = MonthHour(string_to_date(DateTimePicker1.Text))
        FillcomboCommit()
    End Sub

    Private Sub ButtonNewCommit_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ButtonNewCommit.Click
        Dim sql As String
        Dim cmd As New MySqlCommand()

        If ComboBoxCommit.Text <> "HOLIDAY" And ComboBoxCommit.Text <> "UN_COMMIT" And ComboBoxCommit.Text <> "" And TextBoxDescription.Text <> "" And TextBoxOpen.Text <> "" Then
            Try
                sql = "INSERT INTO `" & DBName & "`.`commit_list` (`name` ,`description` ,`open`) VALUES ('" & _
                UCase(ComboBoxCommit.Text) & "', '" & _
                TextBoxDescription.Text & "', '" & _
                TextBoxOpen.Text & "');"
                cmd = New MySqlCommand(sql, MySqlconnection)
                cmd.ExecuteNonQuery()
            Catch ex As Exception
                MsgBox("Insert Error !!")
            End Try
            UpdateTreecommit()
        Else
            MsgBox("Please check input data!")
        End If
        FillcomboCommit()
    End Sub

    ' return the total hour for selected day
    Function dayHour(ByVal d As String) As Integer

        Dim rowShow As DataRow()
        DsCommit.Clear()
        tblCommit.Clear()
        AdapterCommit.Fill(DsCommit, "Commit")
        tblCommit = DsCommit.Tables("Commit")
        rowShow = tblCommit.Select("date= '" & d & IIf(CheckBoxUser.Checked, "' and name='" & ComboBoxUser.Text & "'", "'") & _
        IIf(CheckBoxCommit.Checked, " and commit='" & ComboBoxCommit.Text & "'", ""), "name")
        dayHour = 0
        For Each row In rowShow
            dayHour = dayHour + Val(row("hour"))
        Next
    End Function

    ' return the commit name if for all day. if more commit in the same day return mixed
    Function dayCommit(ByVal d As String) As String

        Dim total As Integer
        Dim rowShow As DataRow()
        DsCommit.Clear()
        tblCommit.Clear()
        AdapterCommit.Fill(DsCommit, "Commit")
        tblCommit = DsCommit.Tables("Commit")
        rowShow = tblCommit.Select("date= '" & d & IIf(CheckBoxUser.Checked, "' and name='" & ComboBoxUser.Text & "'", "'") & _
        IIf(CheckBoxCommit.Checked, " and commit='" & ComboBoxCommit.Text & "'", ""), "name")
        total = 0
        dayCommit = ""
        For Each row In rowShow
            If dayCommit = "" Then
                dayCommit = (row("name"))
            ElseIf dayCommit = (row("name")) Then

            Else
                dayCommit = "MIXED"
            End If
        Next
        TextBoxDay.Text = total
    End Function

    ' is true if there is job with this commit
    Function CommitmentJob(ByVal commit As String) As Boolean

        Dim rowShow As DataRow()
        DsCommit.Clear()
        tblCommit.Clear()
        AdapterCommit.Fill(DsCommit, "Commit")
        tblCommit = DsCommit.Tables("Commit")
        rowShow = tblCommit.Select("commit='" & commit & "'", "name")
        If rowShow.Length > 0 Then
            CommitmentJob = True
        Else
            CommitmentJob = False
        End If
    End Function

    ' total hour in one month based on selection
    Function MonthHour(ByVal QueryDate As Date) As Integer

        Dim rowShow As DataRow()
        DsCommit.Clear()
        tblCommit.Clear()
        AdapterCommit.Fill(DsCommit, "Commit")
        tblCommit = DsCommit.Tables("Commit")
        rowShow = tblCommit.Select("date like '" & Mid(date_to_string(QueryDate), 1, 7) & "*'  " & IIf(CheckBoxUser.Checked, " and name='" & ComboBoxUser.Text & "'", "") & _
                  IIf(CheckBoxCommit.Checked, " and commit='" & ComboBoxCommit.Text & "'", ""), "name")
        MonthHour = 0
        For Each row In rowShow
            MonthHour = MonthHour + Val(row("hour"))
        Next
    End Function


    ' update the table bom offer from the data table offfer
    ' ask to delete bom offer that arent item in offer table
    Sub FillcomboCommit()
        Dim rowResults As DataRow()
        ComboBoxCommit.Items.Clear()
        ComboBoxCommit.Items.Add("")
        ComboBoxCommit.Items.Add("UN_COMMIT")
        ComboBoxCommit.Items.Add("HOLIDAY")
        DsCommitList.Clear()
        tblCommitList.Clear()
        AdapterCommitList.Fill(DsCommitList, "Commit_List")
        tblCommitList = DsCommitList.Tables("Commit_List")
        rowResults = tblCommitList.Select("name like '*'", "name")
        For Each row In rowResults
            ComboBoxCommit.Items.Add(row("name").ToString)
        Next
        ComboBoxCommit.Sorted = True
    End Sub

    ' carico tuti gli user
    Sub FillcomboUser()
        Dim rowResults As DataRow(), commit As String = "", exist As Boolean
        DsCommit.Clear()
        tblCommit.Clear()
        AdapterCommit.Fill(DsCommit, "Commit")
        tblCommit = DsCommit.Tables("Commit")
        rowResults = tblCommit.Select("name like '*'", "name")

        For Each row In rowResults
            If commit <> row("name").ToString Then ComboBoxUser.Items.Add(row("name").ToString)
            commit = row("name").ToString
            If commit = CreAccount.strUserName Then exist = True
        Next
        If Not exist Then ComboBoxUser.Items.Add(CreAccount.strUserName)
        ComboBoxUser.Sorted = True
        ComboBoxUser.Text = CreAccount.strUserName
    End Sub

    ' update the tree viewer
    Sub UpdateTreecommit()
        If CheckBoxMonthView.Checked = False Then
            Dim total As Integer
            TreeViewBomList.Nodes.Clear()
            Dim rootNode As TreeNode
            Dim rowShow As DataRow()
            DsCommit.Clear()
            tblCommit.Clear()
            AdapterCommit.Fill(DsCommit, "Commit")
            tblCommit = DsCommit.Tables("Commit")
            rowShow = tblCommit.Select("date= '" & DateTimePicker1.Text & IIf(CheckBoxUser.Checked, "' and name='" & ComboBoxUser.Text & "'", "' ") & _
            IIf(CheckBoxCommit.Checked, " and commit='" & ComboBoxCommit.Text & "'", ""), "name")
            total = 0
            For Each row In rowShow
                total = total + Val(row("hour"))
                rootNode = New TreeNode(row("id") & " - " & " Hour/s " & row("hour") & "  " & row("commit") & " -- " & row("name"))
                TreeViewBomList.Nodes.Add(rootNode)
            Next
            TextBoxDay.Text = total
        End If
    End Sub

    Private Sub ComboBoxUser_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ComboBoxUser.TextChanged
        If CreAccount.strUserName = ComboBoxUser.Text Or controlRight("R") >= 3 Then
            ButtonNewBom.Enabled = True
            ButtonBomRemove.Enabled = True
        End If
    End Sub


    Private Sub ButtonReset_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ButtonReset.Click
        TextBoxClosed.Text = ""
        Dim sql As String
        Dim cmd As New MySqlCommand()

        Try
            sql = "UPDATE `" & DBName & "`.`commit_list` SET `closed` = '" & TextBoxClosed.Text & "' WHERE `commit_list`.`name` = '" & ComboBoxCommit.Text & "' ;"
            cmd = New MySqlCommand(sql, MySqlconnection)
            cmd.ExecuteNonQuery()
        Catch ex As Exception
            MsgBox("sql Error !!")
        End Try
    End Sub

    Function user(ByVal id As Long)
        Dim rowShow As DataRow()
        DsCommit.Clear()
        tblCommit.Clear()
        AdapterCommit.Fill(DsCommit, "Commit")
        tblCommit = DsCommit.Tables("Commit")
        rowShow = tblCommit.Select("id='" & id & "'", "name")
        If rowShow.Length > 0 Then
            user = rowShow(0).Item("name")
        Else
            user = ""
        End If
    End Function


    Private Sub ComboBoxCommit_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ComboBoxCommit.TextChanged
        Try
            DateTimePicker1.Text = date_to_string(Today)
            TextBoxMontly.Text = MonthHour(string_to_date(DateTimePicker1.Text))
            CheckBoxMonthView.Checked = False
            UpdateTreecommit()
        Catch ex As Exception
        End Try
    End Sub
End Class