window.onload = function () {
    var mime = 'text/x-mssql';
  
    window.editor = CodeMirror.fromTextArea(document.getElementById('code'), {
        mode: mime,
        indentWithTabs: true,
        smartIndent: true,
        lineNumbers: true,
        matchBrackets: true,
        autofocus: true,
        extraKeys: { "Ctrl-Space": "autocomplete" },
        hintOptions: {
            tables: {
                users: { name: null, score: null, birthDate: null },
                countries: { name: null, population: null, size: null }
            }
        }
    });
};

