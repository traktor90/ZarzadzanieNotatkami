// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//function which inserts text in note
function insertText(textToInsert,elementToEdit) {
    var noteText = document.getElementById(elementToEdit);
    var noteTextValue = noteText.value;

    //variable to hold inforamtion where is cursor
    var beginSelStart = noteText.selectionStart;
    //whole new text to insert into edit field
    //it gets old text and put text to insert and then it inserts all text to note
    var newValue = noteTextValue.substring(0, noteText.selectionStart)
        + textToInsert + noteTextValue.substring(noteText.selectionStart);
    noteText.value = newValue;
    //set focus so you can type in same position as before
    noteText.focus();
    //set cursor position so you can start typing in middle of inserted text. 
    noteText.selectionStart = beginSelStart + textToInsert.length / 2;
    noteText.selectionEnd = beginSelStart + textToInsert.length / 2;
}