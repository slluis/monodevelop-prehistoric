<%@ Register TagPrefix="ccms" TagName="PageHeader" src="include/header.ascx" %>
<%@ Register TagPrefix="ccms" TagName="PageFooter" src="include/footer.ascx" %>

<ccms:PageHeader runat="server"/>

<div class="title">Features</div>

<p>MonoDevelop is equipped with lots of features to let you focus on developing your application.  

<div class="feature">
        <b>Class Management</b><br/>
	MonoDevelop has a class viewer which allows you to list the classes in your project, their methods, and properties.  Your namespaces are also kept track of to keep the classes separated.  When you add something to your project, it will automatically be added to the class viewer, even if they're namespaces, classes, methods, or even variables.
      </div>

	<!-- This isn't working in the current release yet ;) -Todd
      <div class="feature">
	<b>Built-in Help</b><br/>
	The .NET documentation and the Gtk# documentation are built into MonoDevelop for easy access.
      </div>

      <div class="feature">
	<b>C# to VB.NET Conversion</b><br>
	Convert your C# code automatically into VB.NET code with the click of a button.
      </div>
      -->

      <div class="feature">
        <b>Code Completion</b><br>
	With the .NET and Gtk# frameworks put together, it can be challenging to remember all the classes, methods, or properties that are at your disposal.  MonoDevelop's intelligent code completion attempts to complete what you're typing.  If it finds a match, just hit tab and MonoDevelop will do the typing for you.
      </div>

      <div class="feature">
        <b>Project Support</b><br>

	MonoDevelop comes with built in projects that help get you started with your console, Gnome# or Gtk# application.
      </div>
    </div>

<ccms:PageFooter runat="server"/>
