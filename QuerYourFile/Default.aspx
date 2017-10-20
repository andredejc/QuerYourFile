<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="QuerYourFile.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script src="lib/codemirror.js"></script>
    <link rel="stylesheet" href="lib/codemirror.css" />
    <link rel="stylesheet" href="stylesheet.css" />
    <link rel="stylesheet" href="https://www.w3schools.com/w3css/4/w3.css" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css" />
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css" />
	<!-- jQuery library -->
	<script src="https://ajax.googleapis.com/ajax/libs/jquery/3.2.1/jquery.min.js"></script>
	<!-- Latest compiled JavaScript -->
	<script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></script>
	<script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/3.2.1/jquery.min.js"></script>
	<script type="text/javascript" src="Scripts/script.js"></script>
    <script src="mode/javascript/javascript.js"></script>
    <script src="mode/sql/sql.js"></script>
    <script src="script.js"></script>
            
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
     <script type="text/javascript">
        function UploadFile(fileUpload) {
            if (fileUpload.value != '') {
                document.getElementById("<%=BtnLoadFile.ClientID %>").click();
            }
        }
    </script>

</head>    
<body>    
    <form id="form1" class="form-inline" runat="server">        
        <div class="container-fluid bg-grey">            
        <nav class="navbar navbar-inverse">
            <div class="container-fluid">
                <div class="navbar-header">
                    <a class="navbar-brand" href="#">QuerYourFile!</a>
                </div>
                <ul class="nav navbar-nav">
                    <li class="active"><a href="#">Home</a></li>                  
                    <li><a href="#">Sobre nós</a></li>
                    <li><a href="#">Contato</a></li>
                </ul>
            </div>
        </nav>
            
            <div class="row">			
                <div class="col-sm-7">
                    
                    <div>
                        <textarea class="form-control" rows="5" id="code" name="code" runat="server"></textarea> 
                        <div>
                            <asp:Label ID="lblError" runat="server" Visible="true"></asp:Label>                            
                        </div>                                    
                    </div>      
                    <div class="row">
                        <div class="col-lg-6">
                            <asp:Button class="btn btn-success btn-sm" id="BtnGO" Font-Bold="true" Text="Vai!" OnClick="BtnGO_Click" runat="server"/>     
                            <asp:Button class="btn btn-warning btn-sm" id="BtnClear" Font-Bold="true" Text="Limpa" OnClick="BtnClear_Click" runat="server"/>                                             

                            <div class="input-group">                                                                 
                                <span class="input-group-btn">                                
                                    <asp:Button class="btn btn-info btn-sm" id="Button2" Text="Salvar" OnClick="BtnSave_Click" runat="server"></asp:Button>
                                </span>
                                <input type="text" id="inpNomeArquivo" class="form-control input-sm" placeholder="Nome do arquivo com extensão:" runat="server" />
                            </div>
                            <div>
                                <br />
                                <label> [ Delimitador ] </label>
                                <select class="form-control" id="sel1">
                                    <option>( , )</option>
                                    <option>( ; )</option>
                                    <option>( tab )</option>                                
                                </select>
                            </div>
                            
                            <asp:Button CssClass="btn btn-info btn-sm" id="BtnLoadFile" Text="Select File" OnClick="BtnUpLoadFile_Click" runat="server" style="display: none" />                                                                                      
                            
                        </div>                     
                    </div>
                    <br />  
                    <div>
                        <asp:Label ID="lblMsg" runat="server" style="font-family:'Consolas'" Visible="false" ForeColor="Green" /> 
                        <asp:FileUpload ID="UpLoad" runat="server" style="font-family:'Consolas'" Font-Size="X-Small" onchange="javascript:UploadFile(this);" />                
                    </div>                                     
                    <br /><br /> 
                                  
                </div>
            </div>
        </div>                   
    </form>         
    <div class="container" runat="server" id="divHtml" ></div>       
</body>
</html>
