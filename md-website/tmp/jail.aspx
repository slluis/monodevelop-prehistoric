<%@ Page Language="C#" %>
<%@ import namespace="System.IO" %>

<html>
<head>
  <title>Does xsp jail me?</title>

<script runat="server">

void Page_Load()
{
  label1.Text = "label text";

  StreamReader sr = null;
  sr = File.OpenText("/home/cityhost/public_html/index.php");

  label1.Text = sr.ReadToEnd();
}
</script>

</head>

<body>

<h2>does it jail me</h2>

<asp:label runat="server" id="label1"></asp:label>

</body>
</html>
