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
                    <li><a href="#sobre">Sobre nós</a></li>
                    <li><a href="#contato">Contato</a></li>            
                </ul>
            </div>

            <div>
                <textarea id="editorSql" spellcheck="false" name="editorSql" runat="server"></textarea>
            </div>

            <div class="container">
                <div class="controles">                    
                    <asp:Button runat="server" id="btn_Vai" Font-Size="X-Small" text="Vai" onclick="BtnVai_Click" /> 
                    <asp:Button runat="server" id="btn_limpa" Font-Size="X-Small" text="Limpa" onclick="BtnLimpa_Click" />                     
                    <asp:Label ID="lblError" runat="server" Visible="true"></asp:Label>     
                </div>

                <div>
                    <fieldset>
                        <legend>Arquivo Entrada:</legend>                                                
                        <label id="lbl_cabecalho">Tem cabeçalho?</label>
                        <select id="sel_TemCabecalho" runat="server">
                            <option value="yes">Sim</option>                            
                            <option value="no">Não</option>
                        </select>
                        

                        <label id="lbl_delimitadorEntrada">Delimitador</label>
                        <select id="sel_delimitadorEntrada" runat="server">
                            <option value="virgula">vírgula</option>
                            <option value="pontovirgula">ponto e vírgula</option>
                            <option value="tabulacao">tabulação</option>
                        </select>  

                        <asp:FileUpload id="UploadDeArquivos" style="font-family:'Consolas'" Font-Size="X-Small" runat="server" />
                        <asp:Button runat="server" id="BtnUploadArquivo" Font-Size="X-Small" text="Upload" onclick="BtnUploadArquivo_Click" />                
                        <asp:Label runat="server" id="StatusLabel" Font-Size="X-Small" text="Status: " />   
                    </fieldset>                 
                </div>

                <div>  
                    <fieldset>
                        <legend>Arquivo Saída:</legend>                        
                        
                        <asp:Button runat="server" id="btn_Salva" Text="Salva" onclick="BtnSalva_Click"/>                        
                        <input type="text" id="inp_nomeArquivo" runat="server" />

                        <label id="lbl_tipoSaida">Tipo do arquivo</label>                                              
                        <asp:DropDownList ID="listaTipoArquivoSaida" runat="server" AutoPostBack="true" OnSelectedIndexChanged="listaTipoArquivoSaida_Selected">
                            <asp:ListItem Value="txt" Text="texto (.txt)" Selected="True" />
                            <asp:ListItem Value="csv" Text="texto (.csv)" />
                            <asp:ListItem Value="dat" Text="texto (.dat)" />
                            <asp:ListItem Value="xlsx" Text="excel (.xlsx)" />
                        </asp:DropDownList>
                      
                        <label id="lbl_delimitadorSaida">Delimitador</label>
                        <asp:DropDownList ID="listaDelimitadorSaida" runat="server"  >
                            <asp:ListItem Value="virgula" Text="Vírgula" Selected="True" />
                            <asp:ListItem Value="ponto_virgula" Text="Ponto e vírgula" />
                            <asp:ListItem Value="tabulacao" Text="tabulação" />
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
