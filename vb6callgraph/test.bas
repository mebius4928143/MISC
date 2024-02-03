Option Explicit
'********************************************************
' �d�b���T���v���\�t�g
'********************************************************
Private DB As Database
Private Const FILENAME = "c:\Database.mdb"

'********************************************************
' �C�x���g�n���h��
'********************************************************
Private Sub AddButton_Click()
    If (Not AddLogic()) Then Exit Sub
    Call ClearData
End Sub

Private Sub CloseButton_Click()
    Call Unload(Me)
End Sub

Private Sub Form_Load()
    Call ConnectDatabase
    CodeTextBox.Text = ""
    Call ClearData
End Sub

Private Sub Form_Unload(Cancel As Integer)
    Call CloseDatabase
End Sub

Private Sub ReadButton_Click()
    Call ReadLogic
End Sub

Private Sub UpdateButton_Click()
    Call UpdateLogic
End Sub

Private Sub DeleteButton_Click()
    If (Not DeleteLogic()) Then Exit Sub
    Call ClearData
End Sub

'********************************************************
' ���O�Ɠd�b�ԍ��̕\�����N���A���܂�
'********************************************************
Private Sub ClearData()
    NameTextBox.Text = ""
    TelephoneNumberTextBox.Text = ""
End Sub

'********************************************************
' �G���[���b�Z�[�W�\��
' ErrorNumber   �G���[�ԍ�
'********************************************************
Private Sub ShowErrorMessage(ByVal ErrorNumber As Long)
    Call MsgBox( _
        CStr(ErrorNumber) & vbCr & Error$(ErrorNumber))
End Sub

'********************************************************
' Code �v���p�e�B
' Returns:  �ԍ�
' Remarks:  CodeTextBox.Text �ɓ��͂���Ă���l�ł�
'********************************************************
Private Property Get Code() As Long
    Code = Val(CodeTextBox.Text)
End Property

Private Property Let Code(ByVal Value As Long)
    CodeTextBox.Text = CStr(Value)
End Property

'********************************************************
' �ǉ����������s���܂�
' Returns:  ���� [True:���� / False:���s]
'********************************************************
Public Function AddLogic() As Boolean
    If (InvalidCode(Code)) Then
        Call MsgBox("�ǉ��ł��܂���ł���")
        AddLogic = False
        Exit Function
    End If
    
    Call AddRecord(Code)

    AddLogic = True
End Function

'********************************************************
' �X�V���������s���܂�
' Returns:  ���� [True:���� / False:���s]
'********************************************************
Private Function UpdateLogic() As Boolean
    If (InvalidCode(Code)) Then
        Call MsgBox("�X�V�Ɏ��s���܂���")
        UpdateLogic = False
        Exit Function
    End If
    
    Call UpdateRecord( _
        Code, _
        NameTextBox.Text, _
        TelephoneNumberTextBox.Text)
    
    UpdateLogic = True
End Function

'********************************************************
' �폜���������s���܂�
' Returns:  ���� [True:���� / False:���s]
'********************************************************
Private Function DeleteLogic() As Boolean
    If (InvalidCode(Code)) Then
        Call MsgBox("�폜�Ɏ��s���܂���")
        DeleteLogic = False
        Exit Function
    End If
    
    Call DeleteRecord(Code)
    
    DeleteLogic = True
End Function

'********************************************************
' �Ǎ��ݏ��������s���܂�
' Returns:  ���� [True:���� / False:���s]
'********************************************************
Private Function ReadLogic() As Boolean
    Dim RS As Recordset
    Set RS = ReadRecord(Code)
    If (RS Is Nothing) Then Exit Function
    If (RS.BOF And RS.EOF) Then
        Call MsgBox("�o�^����Ă��܂���ł���")
        ReadLogic = False
    Else
        NameTextBox.Text = RS("���O")
        TelephoneNumberTextBox.Text = RS("�d�b�ԍ�")
        ReadLogic = True
    End If
    Call RS.Close
End Function

'********************************************************
' �ԍ��l���L�����ǂ��������؂������ʂ��擾���܂�
' Value:    �ԍ��̒l
' Returns:  ���� [True:���� / False:�L��]
'********************************************************
Private Function InvalidCode( _
    ByVal Value As Long) As Boolean

    If (Value <= 0 Or Value >= 100000000) Then
        Call MsgBox("�ԍ��̒l���s���ł�")
        InvalidCode = True
    Else
        InvalidCode = False
    End If
End Function

'********************************************************
' ���R�[�h��ǉ����܂�
' Code:  �ԍ�
' Returns:  ���� [True:���� / False:���s]
'********************************************************
Private Function AddRecord(ByVal Code As Long) As Boolean
    Dim SqlText As String
    SqlText = GetAddSqlText(Code)
    AddRecord = ExecuteQuery(SqlText)
End Function

'********************************************************
' ���R�[�h���X�V���܂�
' Code:          �ԍ�
' NameValue      ���O
' TelephoneNumber   �d�b�ԍ�
' Returns:  ���� [True:���� / False:���s]
'********************************************************
Private Function UpdateRecord( _
    ByVal Code As Long, _
    ByVal NameValue As String, _
    ByVal TelephoneNumber As String _
) As Boolean
    Dim SqlText As String
    SqlText = GetUpdateSqlText( _
        Code, NameValue, TelephoneNumber)
    UpdateRecord = ExecuteQuery(SqlText)
End Function

'********************************************************
' ���R�[�h���폜���܂�
' Code:  �ԍ�
' Returns:  ���� [True:���� / False:���s]
'********************************************************
Private Function DeleteRecord(ByVal Code As Long) As Boolean
    Dim SqlText As String
    SqlText = GetDeleteSqlText(Code)
    DeleteRecord = ExecuteQuery(SqlText)
End Function

'********************************************************
' ���R�[�h��ǂݍ��݂܂�
' Code:  �ԍ�
' Returns:  RecordSet �I�u�W�F�N�g
'********************************************************
Private Function ReadRecord( _
    ByVal Code As Long) As Recordset

    Dim Sql As String
    Sql = GetSelectSqlText(Code)

    On Error Resume Next
    Err.Number = 0

    Dim RS As Recordset
    Set RS = DB.OpenRecordset(Sql, dbOpenSnapshot)
    If (Err.Number <> 0) Then
        Call ShowErrorMessage(Err.Number)
    End If
    On Error GoTo 0

    Set ReadRecord = RS
End Function

'********************************************************
' �e SQL ��������擾���܂�
' Code:  �ԍ�
' Returns:  SQL ������
'********************************************************
Private Function GetSelectSqlText( _
    ByVal Code As Long) As String

    GetSelectSqlText = _
        "SELECT * FROM MyTable " & _
        "WHERE �ԍ� = " & CStr(Code)
End Function

Private Function GetAddSqlText( _
    ByVal Code As Long) As String

    GetAddSqlText = _
        "INSERT INTO MyTable(�ԍ�, ���O, �d�b�ԍ�) " & _
        "VALUES(" & CStr(Code) & ", '' , '')"
End Function

Private Function GetUpdateSqlText( _
    ByVal Code As Long, _
    ByVal NameValue As String, _
    ByVal TelephoneNumber As String _
) As String
    GetUpdateSqlText = _
        "UPDATE MyTable " & _
        "Set " & _
            "���O = '" & NameValue & "', " & _
            "�d�b�ԍ� = '" & TelephoneNumber & "' " & _
        "WHERE �ԍ� = " & CStr(Code)
End Function

Private Function GetDeleteSqlText( _
    ByVal Code As Long) As String

    GetDeleteSqlText = _
        "DELETE * FROM MyTable " & _
        "WHERE �ԍ� = " & CStr(Code)
End Function

'********************************************************
' �N�G���[�����s���܂�
' SqlText:  ���s������ SQL ������
' Returns:  ���� [True:���� / False:���s]
'********************************************************
Private Function ExecuteQuery( _
    ByVal SqlText As String) As Boolean
    
    ExecuteQuery = True
    
    On Error Resume Next
    Err.Number = 0
    Call DB.Execute(SqlText)
    If (Err.Number <> 0) Then
        Call ShowErrorMessage(Err.Number)
        ExecuteQuery = False
    End If
    On Error GoTo 0
End Function

'********************************************************
' �f�[�^�x�[�X�ւ̐ڑ����J�n���܂�
'********************************************************
Public Sub ConnectDatabase()
    Set DB = _
        DBEngine.Workspaces(0).OpenDatabase(FILENAME)
End Sub

'********************************************************
' �f�[�^�x�[�X�̐ڑ����I�����܂�
'********************************************************
Private Sub CloseDatabase()
    Call DB.Close
End Sub

