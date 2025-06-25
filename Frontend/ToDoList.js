const tasksList = document.querySelector('#tasksList');

let tasks = []
let noTasksString = true

let titleInput = document.querySelector('.titleInput')
let descriptionInput = document.querySelector('.descriptionInput')
let deadlineInput = document.querySelector('.deadlineInput')
let priorityInput = document.querySelector('.prioritySelect')

let editTaskbutton = document.querySelector("#editTask")
let addTaskbutton = document.querySelector("#addButton")

let noTasks = document.querySelector('.noTasks')

let modalWindow = document.querySelector('.new_task')
let openModalWindow = document.querySelector('.addTask')
let closeModalWindow = document.querySelector('.close_x')

openModalWindow.addEventListener("click", function(){
    modalWindow.style.display = "block"
})

closeModalWindow.addEventListener("click", function(e){
    modalWindow.style.display = "none"
    e.preventDefault()
})


fetch('http://localhost:5029/tasks', {
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
.then(Tasks => {

    tasks = Tasks

    if (Tasks.length !== 0) noTasksString = false


    tasks.forEach(function (Task){

        let priorityNewTask = Task.priority
        if (priorityNewTask === 0) priorityNewTask = "Low"
        if (priorityNewTask === 1) priorityNewTask = "Medium"
        if (priorityNewTask === 2) priorityNewTask = "High"
        if (priorityNewTask === 3) priorityNewTask = "Critical"

        let cssClass = "task-title"
        if (Task.status === 1 || Task.status === 3) cssClass = "task-title-done"

        let dClass = "deadlineTask"
    
        let deadlineNewTask = Task.deadline

        if (deadlineNewTask === null){
            deadlineNewTask = "Дедлайн не задан"
        } else {
            deadlineNewTask = deadlineNewTask.substring(0, 10)

            const deadline_ch = new Date(deadlineNewTask); 
            const now = new Date();
            const today = new Date(now.getFullYear(), now.getMonth(), now.getDate()); 

            const timeDifference = deadline_ch.getTime() - today.getTime(); 

            const daysDifference = timeDifference / (1000 * 3600 * 24);

            if (daysDifference < 3 && Task.status === 0){
                dClass += " threeDays"
            }
        }

        if (Task.status === 2) dClass += " overdueClass"
    
    
        const taskHTML = `  <li id="${Task.id} "class="list-item">
                                <span class=${cssClass}>${Task.title}</span>
                                <button type="button" data-action="done" class="btn-action-tick"></button>
    
                                <button type="button" data-action="delete" class="btn-action-cross"></button>
    
                                <button type="button" data-action="edit" class="btn-action-edit"></button>
    
                                <div class="info">

                                    <div class="priorityTask">${priorityNewTask}</div>
                                    <div class="${dClass}">${deadlineNewTask}</div>

                                </div>
    
                            </li>`;
        
        tasksList.insertAdjacentHTML('afterbegin', taskHTML);

    
    })

    if (noTasksString){
        noTasks.style.display = "block"
    }
    else{
        noTasks.style.display = "none"
    }
    
})
.catch(error => {
    console.error('Error:', error);
});


addTaskbutton.addEventListener('click', addTask)

tasksList.addEventListener('click', deleteTask)

tasksList.addEventListener('click', doneTask)

tasksList.addEventListener('click', editTask)


function addTask(event){

    modalWindow.style.display = "none"

    event.preventDefault();
    
    let title = titleInput.value

    if (title.length < 4){
        alert("Минимальная длина названия 4 символа")
        modalWindow.style.display = "block"
        return 0;
    }


    let description = descriptionInput.value
    let deadline = deadlineInput.value
    if (deadline !== '') deadline = new Date(deadlineInput.value)
    let priority = null

    for (let i = 0; i < priorityInput.options.length; i++) {

        let option = priorityInput.options[i];

        if (option.selected) {
            if (option.value === "4"){
                priority = null
            }
            else{
                priority = parseInt(option.value);
            }
        }
    }

    if(title === '') title = null
    if(description === '') description = null
    if(deadline === '') deadline = null
    if(priority === '') priority = null

    const newTask = {
        Title: title,
        Description: description,
        Deadline: deadline,
        Priority: priority
    };


    noTasks.style.display = "none"

    fetch('http://localhost:5029/task', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(newTask)
    })
    .then(response => {
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        return response.json();
    })
    .then(Task => {
        tasks.push(Task);

        let priorityNewTask = Task.priority
        if (priorityNewTask === 0) priorityNewTask = "Low"
        if (priorityNewTask === 1) priorityNewTask = "Medium"
        if (priorityNewTask === 2) priorityNewTask = "High"
        if (priorityNewTask === 3) priorityNewTask = "Critical"

        let deadlineNewTask = Task.deadline

        let dClass = "deadlineTask"


        if (deadlineNewTask === null){
            deadlineNewTask = "Дедлайн не задан"
        } else {
            deadlineNewTask = deadlineNewTask.substring(0, 10)
            const deadline_ch = new Date(deadlineNewTask);
            const now = new Date() 
            const today = new Date(now.getFullYear(), now.getMonth(), now.getDate()); 

            const timeDifference = deadline_ch.getTime() - today.getTime();
            console.log(deadline_ch)

            const daysDifference = timeDifference / (1000 * 3600 * 24);

            if (daysDifference < 3 && Task.status === 0){
                dClass += " threeDays"
            }
        }

        if (Task.status === 2) dClass += " overdueClass"

        const taskHTML = `  <li id="${Task.id} "class="list-item">
                                <span class="task-title">${Task.title}</span>
                                <button type="button" data-action="done" class="btn-action-tick"></button>

                                <button type="button" data-action="delete" class="btn-action-cross"></button>

                                <button type="button" data-action="edit" class="btn-action-edit"></button>

                                <div class="info">

                                    <div class="priorityTask">${priorityNewTask}</div>
                                    <div class="${dClass}">${deadlineNewTask}</div>

                                </div>

                            </li>`;
    
        tasksList.insertAdjacentHTML('afterbegin', taskHTML);

        titleInput.value = "";
        descriptionInput.value = "";
        deadlineInput.value = "";
        priorityInput.options[0].selected = true

        console.log(Task)
    })
    .catch(error => {
        console.error('Error:', error);
    });

    
}

function deleteTask(event){

    if (event.target.dataset.action === 'delete'){

        const parenNode = event.target.closest('.list-item')

        const id = parenNode.id

        fetch(`http://localhost:5029/task/${id}`, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json'
            }
        })
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
        })
        .catch(error => {
            console.error('Error:', error);
        });

        const index = tasks.findIndex( function (task){
            if (task.id === id){
                return true
            }
        })

        tasks.splice(index, 1)

        if (tasks.length == 0) noTasks.style.display = "block"

        parenNode.remove()
 
    }


}

function doneTask(event){

    if (event.target.dataset.action === "done"){

        const parentNode = event.target.closest('.list-item')

        const id = parentNode.id

        console.log(tasks)
        console.log(id)

        fetch(`http://localhost:5029/task/complete/${id}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            }
        })
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.json();
        })
        .then(status => {
            console.log(status)

        })
        .catch(error => {
            console.error('Error:', error);
        });

        let taskTitle = parentNode.querySelector('.task-title')
        
        if (taskTitle == null){
            taskTitle = parentNode.querySelector('.task-title-done')
            taskTitle.classList.toggle('task-title-done')
            taskTitle.classList.toggle('task-title')
        } else {
            taskTitle.classList.toggle('task-title-done')
        }

        location.reload()

    }


}

function formatDate(date) {
    let year = date.getFullYear();
    let month = String(date.getMonth() + 1).padStart(2, '0'); // Месяцы начинаются с 0
    let day = String(date.getDate()).padStart(2, '0');

    return `${year}-${month}-${day}`;
}

async function editTask(event){
    if (event.target.dataset.action === "edit"){

        const parenNode = event.target.closest('.list-item')

        const id = parenNode.id

        let task = null

        const response = await fetch(`http://localhost:5029/task/${id}`);
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        task = await response.json();
        console.log(task)



        let date = new Date(task.deadline)
        let formattedDate = formatDate(date)

        modalWindow.style.display = "block"
        addTaskbutton.style.display = "none"
        editTaskbutton.style.display = "block"

        titleInput.value = task.title
        descriptionInput.value = task.description
        deadlineInput.value = formattedDate
        priorityInput.options[task.priority + 1].selected = true


        editTaskbutton.addEventListener("click", function(e){

            e.preventDefault()

            let title = titleInput.value
            let description = descriptionInput.value
            let deadline = deadlineInput.value
            if (deadline !== '') deadline = new Date(deadlineInput.value)
            let priority = null

            for (let i = 0; i < priorityInput.options.length; i++) {

                let option = priorityInput.options[i];

                if (option.selected) {
                    if (option.value === "4"){
                        priority = null
                    }
                    else{
                        priority = parseInt(option.value);
                    }
                }
            }

            if(title === '') title = null
            if(description === '') description = null
            if(deadline === '') deadline = null

            const editsTask = {
                Title: title,
                Description: description,
                Deadline: deadline,
                Priority: priority
            };

            console.log(editsTask)



            fetch(`http://localhost:5029/task/${id}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(editsTask)
            })
            .then(response => {
                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }
                return response.json();
            })
            .then(task => {
                console.log(task)
        
            })
            .catch(error => {
                console.error('Error:', error);
            });

            modalWindow.style.display = "none"
            addTaskbutton.style.display = "block"
            editTaskbutton.style.display = "none"

            titleInput.value = "";
            descriptionInput.value = "";
            deadlineInput.value = "";
            priorityInput.options[0].selected = true

            location.reload();


        })





        // const pareNode = event.target.closest('.list-item')
        // const text = pareNode.querySelector('.task-title')

        // editid = Number(pareNode.id)

        // taskInput.value = text.textContent
        // isEdit = true
        
    }


}

function sortByStatusAndDeadline(tasks) {
    return tasks.sort((a, b) => {
        const priorityA = (a.status === 1 || a.status === 3) ? 1 : -1;
        const priorityB = (b.status === 1 || b.status === 3) ? 1 : -1;

        if (priorityA !== priorityB) {
            return priorityA - priorityB; 
        } else {

            if (a.status === 0 || a.status === 2) {
                if (a.deadline === null && b.deadline === null) {
                    return 0; 
                } else if (a.deadline === null) {
                    return 1; 
                } else if (b.deadline === null) {
                    return -1; 
                } else {

                    const dateA = new Date(a.deadline);
                    const dateB = new Date(b.deadline);
                    return dateA - dateB;
                }
            } else {
                return 0; 
            }
        }
    });
}

function sortByStatusAndPriority(tasks) {
    return tasks.sort((a, b) => {
        const priorityA = (a.status === 1 || a.status === 3) ? 1 : -1;
        const priorityB = (b.status === 1 || b.status === 3) ? 1 : -1;
  
        if (priorityA !== priorityB) {
            return priorityA - priorityB; 
        } else {
            if (a.status === 0 || a.status === 2) {
                return b.priority - a.priority; 
            } else {
                return 0;
            }
        }
    });
}


tasksList.addEventListener('click', function(event) {
    const target = event.target;
    if (target.matches('.todo > li')) {
        const itemId = target.id;
        localStorage.setItem('selectedItemId', itemId);
        window.location.href = 'Task.html';
    }
});

let sortButton = document.querySelector('.sort_button')
let sortSelect = document.querySelector('.sort')

sortButton.addEventListener("click", async function(){
    const sortBy = sortSelect.value;

    const response = await fetch('http://localhost:5029/tasks', {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        },
    })
    if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
    }
    const tasksForSort = await response.json();


    if (sortBy == 1){
        tasks = tasksForSort.sort((a, b) => new Date(a.creationDate) - new Date(b.creationDate));
    }

    if (sortBy == 2){
        tasks = tasksForSort.sort((a, b) => new Date(b.creationDate) - new Date(a.creationDate));
    }

    if (sortBy == 3){
        tasks = sortByStatusAndDeadline(tasksForSort)
        tasks.reverse()
    }

    if (sortBy == 4){
        tasks = sortByStatusAndPriority(tasksForSort)
        tasks.reverse()
    }

    while (tasksList.firstChild) {
        tasksList.removeChild(tasksList.firstChild);
    }

    tasks.forEach(function (Task){

        let priorityNewTask = Task.priority
        if (priorityNewTask === 0) priorityNewTask = "Low"
        if (priorityNewTask === 1) priorityNewTask = "Medium"
        if (priorityNewTask === 2) priorityNewTask = "High"
        if (priorityNewTask === 3) priorityNewTask = "Critical"

        let cssClass = "task-title"
        if (Task.status === 1 || Task.status === 3) cssClass = "task-title-done"

        let dClass = "deadlineTask"
    
        let deadlineNewTask = Task.deadline

        if (deadlineNewTask === null){
            deadlineNewTask = "Дедлайн не задан"
        } else {
            deadlineNewTask = deadlineNewTask.substring(0, 10)

            const deadline_ch = new Date(deadlineNewTask); 
            const now = new Date(); 

            const timeDifference = deadline_ch.getTime() - now.getTime(); 

            const daysDifference = timeDifference / (1000 * 3600 * 24);

            if (daysDifference < 3 && Task.status === 0){
                dClass += " threeDays"
            }
        }

        if (Task.status === 2) dClass += " overdueClass"
    

    
        const taskHTML = `  <li id="${Task.id} "class="list-item">
                                <span class=${cssClass}>${Task.title}</span>
                                <button type="button" data-action="done" class="btn-action-tick"></button>
    
                                <button type="button" data-action="delete" class="btn-action-cross"></button>
    
                                <button type="button" data-action="edit" class="btn-action-edit"></button>
    
                                <div class="info">

                                    <div class="priorityTask">${priorityNewTask}</div>
                                    <div class="${dClass}">${deadlineNewTask}</div>

                                </div>
    
                            </li>`;
        
        tasksList.insertAdjacentHTML('afterbegin', taskHTML);

    
    })

    if (noTasksString){
        noTasks.style.display = "block"
    }
    else{
        noTasks.style.display = "none"
    }
})

