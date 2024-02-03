Option Explicit
'********************************************************
' 電話帳サンプルソフト
'********************************************************
Private DB As Database
Private Const FILENAME = "c:\Database.mdb"

'********************************************************
' イベントハンドラ
'********************************************************
Private Sub AddButton2_Click()
    If (Not AddLogic()) Then Exit Sub
    Call ClearData
End Sub

Private Sub CloseButton2_Click()
    Call Unload(Me)
End Sub

Private Sub Form_Load2()
    Call test.ConnectDatabase
    CodeTextBox.Text = ""
    Call ClearData
End Sub

Public Sub SubPublic()
    CodeTextBox.Text = ""
    Call Form_Load2
End Sub

