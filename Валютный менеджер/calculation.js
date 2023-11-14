let valutaElementFirst = null;
let valutaElementSecond = null;
let searchValutaInput = null;
let responseText;
window.addEventListener("DOMContentLoaded", () => {
    document.getElementById('calendar').valueAsDate = new Date();
    GetActualValuta();
    document.getElementById('calendar').addEventListener('input', Redate)
    valutaElementFirst = document.getElementsByClassName("element-menu-valuta first-menu");
    valutaElementSecond = document.getElementsByClassName("element-menu-valuta second-menu");
    searchValutaInput = document.getElementsByClassName("input-search-valuta");
    document.getElementById("button-select-valuta-first").addEventListener('click', ButtonSelectFirst);
    document.getElementById("button-select-valuta-second").addEventListener('click', ButtonSelectSecond);
    document.getElementById("input-valuta-first").addEventListener('input', InputMoneyFirst);
    document.getElementById("input-valuta-second").addEventListener('input', InputMoneySecond);
    document.getElementById("button-valuta-swap").addEventListener('click', BtnValutaSwap);
    searchValutaInput[0].addEventListener('input', SearchValutaFirst);
    searchValutaInput[1].addEventListener('input', SearchValutaSecond);
    for (let i = 0; i < valutaElementFirst.length; i++){
        valutaElementFirst[i].addEventListener('click', ChoiceValutaFirst)
    }
    for (let i = 0; i < valutaElementSecond.length; i++){
        valutaElementSecond[i].addEventListener('click', ChoiceValutaSecond)
    }
    /*document.addEventListener('click', function() {
        var eone = document.getElementById('menu-select-valuta-first');
        if (eone.style.display = "block"){
            eone.style.display='none';
        }
        var etwo = document.getElementById('menu-select-valuta-second');
        if (etwo.style.display = "block"){
            etwo.style.display='none';
        }
    });*/
});
function ButtonSelectFirst(){
    let firstMenu = document.getElementById("menu-select-valuta-first");
    let firstButtonMenu = document.getElementById("button-select-valuta-first");
    if (firstMenu.style.display == "none"){
        firstMenu.style.display = "block";
        firstButtonMenu.className = "button-to-select-valuta-open"
    }
    else{
        firstMenu.style.display = "none";
        firstButtonMenu.className = "button-to-select-valuta-close"
    }
}
function ButtonSelectSecond(){
    let secondMenu = document.getElementById("menu-select-valuta-second");
    let firstButtonMenu = document.getElementById("button-select-valuta-second");
    if (secondMenu.style.display == "none"){
        secondMenu.style.display = "block";
        firstButtonMenu.className = "button-to-select-valuta-open"
    }
    else{
        secondMenu.style.display = "none";
        firstButtonMenu.className = "button-to-select-valuta-close"
    }
}
function BtnValutaSwap(){
    const a = document.getElementById("input-valuta-first").value;
    const b = document.getElementById("button-select-valuta-first").innerText;
    document.getElementById("input-valuta-first").value = document.getElementById("input-valuta-second").value;
    document.getElementById("input-valuta-second").value = a;
    document.getElementById("button-select-valuta-first").innerText = document.getElementById("button-select-valuta-second").innerText; 
    document.getElementById("button-select-valuta-second").innerText = b;
}
async function ChoiceValutaFirst(){ 
    let date = document.getElementById('calendar').valueAsDate;
    let valutaFirst = document.getElementById("button-select-valuta-first");
    let valutaSecond = document.getElementById("button-select-valuta-second").innerText;
    let target = event.target;
    if (target.className == 'element-menu-valuta first-menu'){
        valutaFirst.innerText = target.childNodes[1].innerText;
    }
    else if (target.className == 'valuta-code first-menu'){
        valutaFirst.innerText = target.innerText;
    }
    else if (target.className == 'valuta-name first-menu'){
        valutaFirst.innerText = target.previousSibling.previousSibling.innerText;
    }
    if (valutaSecond != 'ВЫБРАТЬ'){
        let requestCreateBody = {
            first: valutaFirst.innerText,
            second: valutaSecond,
            date: String(`${date.getFullYear()}/${date.getMonth() + 1}/${date.getDate()}`)
            };
        let response = await fetch('http://localhost:8080', {method: 'POST', headers: {'Content-Type': 'text/plain'}, body: JSON.stringify(requestCreateBody)});
        responseText = await response.text();
        responseText = responseText.split(",").join(".").split("&");
    } 
    InputMoneyFirst();
}
async function ChoiceValutaSecond(){  
    let date = document.getElementById('calendar').valueAsDate;
    let valutaFirst = document.getElementById("button-select-valuta-first").innerText;
    let valutaSecond = document.getElementById("button-select-valuta-second");  
    let target = event.target;
    if (target.className == 'element-menu-valuta second-menu'){
        valutaSecond.innerText = target.childNodes[1].innerText;
    }
    else if (target.className == 'valuta-code second-menu'){
        valutaSecond.innerText = target.innerText;
    }
    else if (target.className == 'valuta-name second-menu'){
        valutaSecond.innerText = target.previousSibling.previousSibling.innerText;
    }
    if (valutaFirst != 'ВЫБРАТЬ'){
        let requestCreateBody = {
            first: valutaFirst,
            second: valutaSecond.innerText,
            date: String(`${date.getFullYear()}/${date.getMonth() + 1}/${date.getDate()}`)
            };
        let response = await fetch('http://localhost:8080', {method: 'POST', headers: {'Content-Type': 'text/plain'}, body: JSON.stringify(requestCreateBody)});
        responseText = await response.text();
        responseText = responseText.split(",").join(".").split("&");
    } 
    if (document.getElementById("input-valuta-second").value == ""){
        InputMoneySecond();
    }
    else{
        InputMoneyFirst();
    }
}
async function InputMoneyFirst(){
    let valutaFirst = document.getElementById("button-select-valuta-first").innerText;
    let valutaSecond = document.getElementById("button-select-valuta-second").innerText;
    if (valutaFirst != 'ВЫБРАТЬ' && valutaSecond != 'ВЫБРАТЬ'){
        if (document.getElementById("input-valuta-first").value !== ""){
            let count = document.getElementById("input-valuta-first").value;
            document.getElementById("input-valuta-second").value = Math.trunc(responseText[0] * count / responseText[1] * 10000) / 10000;
        }
        else if (document.getElementById("input-valuta-second").value !== ""){
            let count = document.getElementById("input-valuta-second").value;
            document.getElementById("input-valuta-first").value = Math.trunc(responseText[1] * count / responseText[0] * 10000) / 10000;
        }
        DrawDiagram(valutaFirst, valutaSecond, responseText[0] / responseText[1], responseText[1] / responseText[0]);
    }
}
async function InputMoneySecond(){
    let valutaFirst = document.getElementById("button-select-valuta-first").innerText;
    let valutaSecond = document.getElementById("button-select-valuta-second").innerText;
    if (valutaFirst != 'ВЫБРАТЬ' && valutaSecond != 'ВЫБРАТЬ'){
        if (document.getElementById("input-valuta-second").value !== ""){
            let count = document.getElementById("input-valuta-second").value;
            document.getElementById("input-valuta-first").value = Math.trunc(responseText[1] * count / responseText[0] * 10000) / 10000;
        }
        else if (document.getElementById("input-valuta-first").value !== ""){
            let count = document.getElementById("input-valuta-first").value;
            document.getElementById("input-valuta-second").value = Math.trunc(responseText[0] * count / responseText[1] * 10000) / 10000;
        }
        DrawDiagram(valutaFirst, valutaSecond, responseText[0] / responseText[1], responseText[1] / responseText[0]);
    }
}
function SearchValutaFirst(){
    let substring = event.target.value;
    for (let i = 0; i < valutaElementFirst.length; i++){
        if (!valutaElementFirst[i].innerText.toLowerCase().includes(substring.toLowerCase())){
            valutaElementFirst[i].style.display = "none";
            valutaElementFirst[i].previousSibling.previousSibling.style.display = "none";
        }
        else{
            valutaElementFirst[i].style.display = "flex";
            valutaElementFirst[i].previousSibling.previousSibling.style.display = "block";
        }
    }
}
function SearchValutaSecond(){
    let substring = event.target.value;
    for (let i = 0; i < valutaElementSecond.length; i++){
        if (!valutaElementSecond[i].innerText.toLowerCase().includes(substring.toLowerCase())){
            valutaElementSecond[i].style.display = "none";
            valutaElementSecond[i].previousSibling.previousSibling.style.display = "none";
        }
        else{
            valutaElementSecond[i].style.display = "flex";
            valutaElementSecond[i].previousSibling.previousSibling.style.display = "block";
        }
    }
}
async function Redate(){
    let date = document.getElementById('calendar').valueAsDate;
    let valutaFirst = document.getElementById("button-select-valuta-first").innerText;
    let valutaSecond = document.getElementById("button-select-valuta-second").innerText;
    if (valutaFirst != 'ВЫБРАТЬ' && valutaSecond != 'ВЫБРАТЬ'){
        let requestCreateBody = {
            first: valutaFirst,
            second: valutaSecond,
            date: String(`${date.getFullYear()}/${date.getMonth() + 1}/${date.getDate()}`)
            };
        let response = await fetch('http://localhost:8080', {method: 'POST', headers: {'Content-Type': 'text/plain'}, body: JSON.stringify(requestCreateBody)});
        responseText = await response.text();
        responseText = responseText.split(",").join(".").split("&");
        GetActualValuta();
        InputMoneyFirst();
    }
}
async function GetActualValuta(){
    let date = document.getElementById('calendar').valueAsDate;
    let countValutaNow = document.getElementsByClassName("rate-main");
    let requestCreateBody = {
        text: `GetActualValuta:${countValutaNow.length}`,
        date: String(`${date.getFullYear()}/${date.getMonth() + 1}/${date.getDate()}`)
        };
    let response = await fetch('http://localhost:8080/', {method: 'POST', headers: {'Content-Type': 'text/plain'}, body: JSON.stringify(requestCreateBody)});
    let responseText = await response.text();
    responseText = responseText.split(",").join(".").split("/");
    for (let i = 0; i < countValutaNow.length; i++){
        countValutaNow[i].innerText = responseText[i];
    }
    DrawDiagram("RUB", "USD", (1 / responseText[38]), (responseText[38] / 1))
}
async function DrawDiagram(firstValuta, secondValuta, firstValue, secondValue){
    const ctx = document.getElementById('my-chart-diagram');
    let ch = new Chart(ctx, {
        type: 'line',
        data: {
            labels: [""],
            datasets: [{
                label: 'Курс '+ firstValuta + " от " + secondValuta,
                data: [firstValue],
                borderWidth: 1
            },
            {
                label: 'Курс '+ secondValuta + " от " + firstValuta,
                data: [secondValue],
                borderWidth: 1
            }]
        },
        options: {}
    });
}