<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="QuerYourFile.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>QuerYourFile</title>
    <link rel="stylesheet" type="text/css" href="index.css" />   
</head>    
<body>    
    <form id="form1" class="form-inline" runat="server">
        <div class="box">
            <div>
                <ul>
                    <li><a class="active">QuerYourFile</a></li>                             
                </ul>
            </div>

            <div>
                <textarea id="editorSql" spellcheck="false" name="editorSql" runat="server"></textarea>
            </div>

            <div class="container">
                <div class="controles">                    
                    <asp:Button runat="server" id="btn_Vai" Font-Size="X-Small" onclick="BtnVai_Click" /> 
                    <asp:Button runat="server" id="btn_Limpa" Font-Size="X-Small" onclick="BtnLimpa_Click" />                     
                    <asp:Label ID="lblError" runat="server" Visible="true"></asp:Label>     
                </div>

                <div>
                    <fieldset>
                        <legend id ="leg_ArquivoEntrada" runat="server"></legend>                                                
                        <label id="lbl_Cabecalho" runat="server"></label>
                        <asp:DropDownList ID="sel_TemCabecalho" runat="server"  >
	                        <asp:ListItem Value="<%$ Resources:ResourceFile, stringYes %>" Text="<%$ Resources:ResourceFile, yes %>" Selected="True" />
	                        <asp:ListItem Value="<%$ Resources:ResourceFile, stringNo %>" Text="<%$ Resources:ResourceFile, no %>" />	
                        </asp:DropDownList>                        

                        <label id="lbl_DelimitadorEntrada" runat="server"></label>
                        <asp:DropDownList ID="sel_delimitadorEntrada" runat="server"  >
	                        <asp:ListItem Value="<%$ Resources:ResourceFile, listVirgula %>" Text="<%$ Resources:ResourceFile, virgula %>" Selected="True" />
	                        <asp:ListItem Value="<%$ Resources:ResourceFile, listPontoVirgula %>" Text="<%$ Resources:ResourceFile, ponto_virgula %>" />
	                        <asp:ListItem Value="<%$ Resources:ResourceFile, listTabulacao %>" Text="<%$ Resources:ResourceFile, tabulacao %>" />
                        </asp:DropDownList> 

                        <asp:FileUpload id="UploadDeArquivos" style="font-family:'Consolas'" Font-Size="X-Small" runat="server" />
                        <asp:Button runat="server" id="BtnUploadArquivo" Font-Size="X-Small" text="<%$ Resources:ResourceFile, stringUpload %>" onclick="BtnUploadArquivo_Click" />                
                        <asp:Label runat="server" id="StatusLabel" Font-Size="X-Small" text="Status: " />   
                    </fieldset>                 
                </div>

                <div>  
                    <fieldset>
                        <legend id="leg_ArquivoSaida" runat="server"></legend>                          
                        <asp:Button runat="server" id="btn_Salva" Text="Salva" onclick="BtnSalva_Click"/>                        
                        <input type="text" id="inp_nomeArquivo" runat="server" />

                        <label id="lbl_TipoSaida" runat="server"></label>                                              
                        <asp:DropDownList ID="listaTipoArquivoSaida" runat="server" AutoPostBack="true" OnSelectedIndexChanged="listaTipoArquivoSaida_Selected">
                            <asp:ListItem Value="<%$ Resources:ResourceFile, listTxt %>" Text="<%$ Resources:ResourceFile, txt %>" Selected="True" />
                            <asp:ListItem Value="<%$ Resources:ResourceFile, listCsv %>" Text="<%$ Resources:ResourceFile, csv %>" />
                            <asp:ListItem Value="<%$ Resources:ResourceFile, listDat %>" Text="<%$ Resources:ResourceFile, dat %>" />
                            <asp:ListItem Value="<%$ Resources:ResourceFile, listXlsx %>" Text="<%$ Resources:ResourceFile, xlsx %>" />
                        </asp:DropDownList>
                      
                        <label id="lbl_DelimitadorSaida" runat="server"></label>
                        <asp:DropDownList ID="listaDelimitadorSaida" runat="server"  >
                            <asp:ListItem Value="<%$ Resources:ResourceFile, listVirgula %>" Text="<%$ Resources:ResourceFile, virgula %>" Selected="True" />
                            <asp:ListItem Value="<%$ Resources:ResourceFile, listPontoVirgula %>" Text="<%$ Resources:ResourceFile, ponto_virgula %>" />
                            <asp:ListItem Value="<%$ Resources:ResourceFile, listTabulacao %>" Text="<%$ Resources:ResourceFile, tabulacao %>" />
                        </asp:DropDownList>
                    </fieldset>            
                </div>                                                                                                                         
            </div>
        </div>
    </form>
    <br />
    <div class="tabela" runat="server" id="divHtml" ></div>
</body>
</html>
