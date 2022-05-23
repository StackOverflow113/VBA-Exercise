Imports System.Data.OleDb
Imports System.Text.RegularExpressions

Public Class ClienteForm

  Dim dtCliente As DataTable

  Public Sub Llenar()
    Dim cadena As String = "provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & My.Application.Info.DirectoryPath & "\Database.xls" & ";Extended Properties=Excel 8.0;"
    Dim conn As New OleDbConnection(cadena)
    conn.Open()

    Dim da As New OleDbDataAdapter("select * from [Hoja1$]", conn)
    Dim ds As New DataSet
    da.Fill(ds)

    dtCliente = New DataTable
    dtCliente = ds.Tables(0)

    Dim bs As New BindingSource
    bs.DataSource = dtCliente

    If IsNumeric(txtBuscar.Text) Then
      bs.Filter = "CODE like '%" & txtBuscar.Text & "%'"
    Else
      bs.Filter = "NAME like '%" & txtBuscar.Text & "%'"
    End If


    DataGridView1.DataSource = bs
    conn.Close()
  End Sub
  Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
    Llenar()
    Format(DateTimePicker1.Text, "MM/dd/yyyy")
  End Sub

  Private Sub LlenaTexto()
    txtID.Text = DataGridView1.CurrentRow.Cells("CODE").Value.ToString
    txtNombre.Text = DataGridView1.CurrentRow.Cells("NAME").Value.ToString
    DateTimePicker1.Text = DataGridView1.CurrentRow.Cells("DATEBIRTH").Value.ToString
    txtEmail.Text = DataGridView1.CurrentRow.Cells("EMAIL").Value.ToString
    txtDireccion.Text = DataGridView1.CurrentRow.Cells("ADDRESS").Value.ToString

  End Sub

  Public Sub LimpiarCampos()
    txtID.Clear()
    txtNombre.Clear()
    txtEmail.Clear()
    txtDireccion.Clear()

    txtNombre.Focus()

  End Sub

  Private Sub CrearID()
    Dim ValorID As Integer = 0

    For Each Fila As DataRow In dtCliente.Rows
      If IsNumeric(Fila("CODE")) = True Then
        If CInt(Fila("CODE")) > CInt(ValorID) Then
          ValorID = Fila("CODE")
        End If

      End If
    Next

    ValorID += 1
    txtID.Text = ValorID
    txtNombre.Focus()
  End Sub

  Private Sub btnEliminar_Click(sender As Object, e As EventArgs) Handles btnEliminar.Click

    If MsgBox("Desea eliminar al usuario" + " " + txtNombre.Text, vbQuestion + vbYesNo, "Eliminar usuario") = vbYes Then
      If Me.txtID.Text = "" Then
        MsgBox("No hay registro para eliminar.", vbCritical + vbOKOnly, "Sin registro")
      Else
        Dim cadena As String = "provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & My.Application.Info.DirectoryPath & "\Database.xls" & ";Extended Properties=Excel 8.0;"
        Dim conn As New OleDbConnection(cadena)
        conn.Open()

        Dim cmd As New OleDbCommand("update [Hoja1$] set CODE='',NAME='',DATEBIRTH='',EMAIL='',ADDRESS='' where CODE='" & txtID.Text & "'", conn)
        cmd.ExecuteNonQuery()
        MsgBox("Usuario eliminado correctamente", vbInformation + vbOKOnly, "Exito")
        conn.Close()
        Llenar()
        LimpiarCampos()
      End If
    Else
      LimpiarCampos()
      Exit Sub
    End If
  End Sub

  Private Sub btnActualizar_Click(sender As Object, e As EventArgs) Handles btnActualizar.Click

    If MsgBox("Desea guardar los cambios?", vbQuestion + vbYesNo, "Guardar cambios") = vbYes Then

      If Me.txtID.Text = "" Then
        MsgBox("No existe registro activo.", vbCritical + vbOKOnly, "Sin registro")
      Else
        Dim cadena As String = "provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & My.Application.Info.DirectoryPath & "\Database.xls" & ";Extended Properties=Excel 8.0;"

        Dim conn As New OleDbConnection(cadena)
        conn.Open()
        Dim cmd As New OleDbCommand("update [Hoja1$] set NAME='" & txtNombre.Text & "',DATEBIRTH='" & DateTimePicker1.Value & "',EMAIL='" & txtEmail.Text & "',ADDRESS='" & txtDireccion.Text & "' WHERE CODE='" & txtID.Text & "'", conn)
        cmd.ExecuteNonQuery()
        conn.Close()
        MsgBox("Cambios guardados correctamente", vbInformation + vbOKOnly, "Exito")
        Llenar()
        LimpiarCampos()
      End If

    Else
      LimpiarCampos()
      Exit Sub

    End If

  End Sub

  Private Sub btnGuardar_Click(sender As Object, e As EventArgs) Handles btnGuardar.Click
    If Me.txtID.Text = "" Then

      MsgBox("No existe registro activo.", vbCritical + vbOKOnly, "Sin registro")

    Else

      Dim cadena As String = "provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & My.Application.Info.DirectoryPath & "\Database.xls" & ";Extended Properties=Excel 8.0;"
      Dim conn As New OleDbConnection(cadena)
      conn.Open()

      If Validation() = True Then
        Exit Sub
      Else
        Dim bs As New BindingSource
        bs.DataSource = dtCliente
        bs.Filter = "Code ='0'"

        If bs.Count > 0 Then
          Dim cmd As New OleDbCommand("update [Hoja1$] set CODE='" & txtID.Text & "',NAME='" & txtNombre.Text & "',DATEBIRTH='" & DateTimePicker1.Value & "',EMAIL='" & txtEmail.Text & "',ADDRESS='" & txtDireccion.Text & "' where Code='0'", conn)
          cmd.ExecuteNonQuery()
          LimpiarCampos()
        Else
          Dim cmd As New OleDbCommand(" INSERT INTO [Hoja1$] (CODE,NAME,DATEBIRTH,EMAIL,ADDRESS) values('" & txtID.Text & "','" & txtNombre.Text & "','" & Format(DateTimePicker1.Value, "MM/dd/yyyy") & "', '" & txtEmail.Text & "', '" & txtDireccion.Text & "')", conn)
          cmd.ExecuteNonQuery()
          MsgBox("Usuario creado correctamente", vbInformation + vbOKOnly, "Exito")
          LimpiarCampos()
        End If

      End If

      conn.Close()
      Llenar()
      Me.btnEliminar.Enabled = True
      Me.btnActualizar.Enabled = True
    End If

  End Sub

  Private Sub btnNuevo_Click(sender As Object, e As EventArgs) Handles btnNuevo.Click
    LimpiarCampos()
    CrearID()
    Me.btnEliminar.Enabled = False
    Me.btnActualizar.Enabled = False
  End Sub

  Private Sub txtBuscar_TextChanged(sender As Object, e As EventArgs) Handles txtBuscar.TextChanged
    Llenar()
  End Sub

  Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
    If DataGridView1.RowCount > 0 Then
      LlenaTexto()
    Else
      LimpiarCampos()
    End If
  End Sub

  Private Function Validation() As Boolean
    Validation = False

    If Me.txtNombre.Text = "" Then
      MsgBox("El campo no puede estar vacio.", vbExclamation + vbOKOnly, "Campo vacio")
      txtNombre.Focus()
      Validation = True
      Exit Function
    End If

    If txtEmail.Text = "" Then
      MsgBox("El campo esta vacio.", vbExclamation + vbOKOnly, "Sin datos")
      Validation = True
      Me.txtEmail.Focus()
      Exit Function
    End If

    If Me.txtID.Text = "" Then
      MsgBox("Si quiere agregar un nuevo usuario de click en nuevo.", vbExclamation + vbOKOnly, "Sin id")
      Me.btnNuevo.Focus()
      Validation = True
      Exit Function
    End If

    Dim correo As String
    correo = “\w+([-+.’]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*”
    If Regex.IsMatch(txtEmail.Text, correo) = False Then
      MsgBox("El formato ingresado no es valido, verificar.", vbExclamation + vbOKOnly, "Correo Invalido")
      Validation = True
      Me.txtEmail.Focus()
      Exit Function
    End If

    If Me.DateTimePicker1.Text = Date.Today.ToShortDateString Then
      MsgBox("Verificar su fecha de nacimiento por favor.", vbExclamation + vbOKOnly, "Fecha Incorrecta")
      Validation = True
      Me.DateTimePicker1.Focus()
      Exit Function
    End If

    If Me.DateTimePicker1.Value > Date.Today.ToShortDateString Then
      MsgBox("Su fecha de nacimiento no puede ser mayor al dia de hoy.", vbExclamation + vbOKOnly, "Fecha Incorrecta")
      Validation = True
      Me.DateTimePicker1.Focus()
      Exit Function
    End If

  End Function

End Class
