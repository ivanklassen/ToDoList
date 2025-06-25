const id = localStorage.getItem('selectedItemId')
console.log(id)

const close_info = document.querySelector('.close_info')

const title_info = document.querySelector('.title_info')
const description_info = document.querySelector('.description_info')

const d_info = document.querySelector('#d_value')
const s_info = document.querySelector('#s_value')
const p_info = document.querySelector('#p_value')
const dc_info = document.querySelector('#dc_value')
const de_info = document.querySelector('#de_value')

close_info.addEventListener('click', function(){
    window.location.href = "index.html"
})

function formatDate(date) {
    let year = date.getFullYear();
    let month = String(date.getMonth() + 1).padStart(2, '0'); // Месяцы начинаются с 0
    let day = String(date.getDate()).padStart(2, '0');

    return `${year}-${month}-${day}`;
}

fetch(`http://localhost:5029/task/${id}`, {
    method: 'GET',
    headers: {
        'Content-Type': 'application/json'
    },
})
.then(response => {
    if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
    }
    return response.json();
})
.then(task => {

    console.log(task)

    title_info.textContent = task.title

    if (task.description !== null){
        description_info.textContent = task.description
    }

    if (task.deadline !== null){
        let date = new Date(task.deadline)
        let formattedDate = formatDate(date)
        d_info.textContent = formattedDate
    }

    if (task.status === 0) s_info.textContent = "Active"
    if (task.status === 1) s_info.textContent = "Completed"
    if (task.status === 2) s_info.textContent = "Overdue"
    if (task.status === 3) s_info.textContent = "Late"

    if (task.priority === 0) p_info.textContent = "Low"
    if (task.priority === 1) p_info.textContent = "Medium"
    if (task.priority === 2) p_info.textContent = "High"
    if (task.priority === 3) p_info.textContent = "Critical"

    dc_info.textContent = task.creationDate.substring(0, 10);

    if (task.editingDate !== null) de_info.textContent = task.editingDate.substring(0, 10);

    
})
.catch(error => {
    console.error('Error:', error);
});



