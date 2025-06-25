describe('Создание задач', () => {
    beforeEach(() => {
        cy.visit('/'); 
    });
    afterEach(() => {
        cy.wait(1000);
    });

    it('Нельзя создать задачу с 3 символами', () => {
        cy.get('.addTask').click();
        const todoText = Math.random().toString(36).substring(2, 5);
        cy.get('#taskInput').type(todoText);
        cy.get('#addButton').click();

        cy.get('#tasksList li').should('have.length', 0);
    });

    it('Можно создать задачу с 4 символами', () => {
        cy.get('.addTask').click();
        const todoText = Math.random().toString(36).substring(2, 6);
        cy.get('#taskInput').type(todoText);
        cy.get('#addButton').click();

        cy.get('.todo li').should('have.length', 1);
        cy.get('.todo li span').should('contain', todoText);
  });

})

describe('Редактирование задачи', () => {
    beforeEach(() => {
        cy.visit('/'); 
    });

    it('Изменение всех атрибутов, установление дедлайна на завтра', () => {
        cy.get('.todo li .btn-action-edit').click();
        const todoText = Math.random().toString(36).substring(2, 6);
        cy.get('#taskInput').type(todoText);
        cy.get('.descriptionInput').type("description");

        const today = new Date();
        const tomorrow = new Date(today);
        tomorrow.setDate(today.getDate() + 1);
        const tomorrowFormatted = tomorrow.toISOString().split('T')[0];

        cy.get('.deadlineInput').type(tomorrowFormatted);

        cy.get('#editTask').click();

        cy.get('.todo li span').should('contain', todoText);
        cy.get('.threeDays').should('have.css', 'background-color', 'rgb(224, 148, 62)');
        cy.get('.todo li').click();
        cy.get('.status-value').should('contain', 'Active');

    });

    it('Установление дедлайна на вчера', () => {
        cy.get('.todo li .btn-action-edit').click();

        const today = new Date();
        const tomorrow = new Date(today);
        tomorrow.setDate(today.getDate() - 1);
        const tomorrowFormatted = tomorrow.toISOString().split('T')[0];

        cy.get('.deadlineInput').type(tomorrowFormatted);

        cy.get('#editTask').click();

        cy.get('.overdueClass').should('have.css', 'background-color', 'rgb(231, 78, 78)');
        cy.get('.todo li').click();
        cy.get('.status-value').should('contain', 'Overdue');

    });

    it('Отметить задачу как выполненную', () => {
        cy.get('.todo li .btn-action-tick').click();
        cy.get('.todo li').click();
        cy.get('.status-value').should('contain', 'Late');

    });

    it('Отметить задачу как невыполненную', () => {
        cy.get('.todo li .btn-action-tick').click();
        cy.get('.todo li').click();
        cy.get('.status-value').should('contain', 'Overdue');

    });

    it('Отметить задачу как выполненную', () => {
        cy.get('.todo li .btn-action-tick').click();
        cy.get('.todo li').click();
        cy.get('.status-value').should('contain', 'Late');

    });

    it('Установление дедлайна на конец месяца', () => {
        cy.get('.todo li .btn-action-edit').click();

        const today = new Date();
        const tomorrow = new Date(today);
        tomorrow.setDate(today.getDate() + 7);
        const tomorrowFormatted = tomorrow.toISOString().split('T')[0];

        cy.get('.deadlineInput').type(tomorrowFormatted);

        cy.get('#editTask').click();

        cy.get('.deadlineTask').should('have.css', 'background-color', 'rgb(216, 228, 248)');
        cy.get('.todo li').click();
        cy.get('.status-value').should('contain', 'Completed');

    });

})

describe('Создание задач с макросами', () => {
    beforeEach(() => {
        cy.visit('/'); 
    });
    afterEach(() => {
        cy.wait(1000);
    });

    it('Создание задачи с макросом приоритета', () => {
        cy.get('.addTask').click();
        const Text = Math.random().toString(36).substring(2, 6);
        const todoText = `!1 ${Text}`;
        cy.get('#taskInput').type(todoText);
        cy.get('#addButton').click();

        cy.get('ul li')
            .contains(Text) 
            .parents('li')    
            .find('.priorityTask') 
            .should('have.text', 'Critical'); 

    });

    it('Создание задачи с макросом дедлайна', () => {
        cy.get('.addTask').click();
        const Text = Math.random().toString(36).substring(2, 6);
        const todoText = `!before 30.05.2025 ${Text}`;
        cy.get('#taskInput').type(todoText);
        cy.get('#addButton').click();

        cy.get('ul li')
            .contains(Text) 
            .parents('li')    
            .find('.deadlineTask') 
            .should('have.text', '2025-05-30'); 

    });

    it('Создание задачи с обоими макросами', () => {
        cy.get('.addTask').click();
        const Text = Math.random().toString(36).substring(2, 6);
        const todoText = `!2 !before 29.05.2025 ${Text}`;
        cy.get('#taskInput').type(todoText);
        cy.get('#addButton').click();

        cy.get('ul li')
            .contains(Text) 
            .parents('li')    
            .find('.deadlineTask') 
            .should('have.text', '2025-05-29'); 
        
        cy.get('ul li')
            .contains(Text) 
            .parents('li')    
            .find('.priorityTask') 
            .should('have.text', 'High'); 

    });

})

describe('Создание задачи с явным указанием параметров', () => {
    beforeEach(() => {
        cy.visit('/'); 
    });
    afterEach(() => {
        cy.wait(1000);
    });

    it('Создание задачи с обоими макросами', () => {
        cy.get('.addTask').click();
        const Text = Math.random().toString(36).substring(2, 6);
        const todoText = `!2 !before 29.05.2025 ${Text}`;
        cy.get('#taskInput').type(todoText);
        cy.get('.descriptionInput').type("description");
        cy.get('.deadlineInput').type('2025-05-25');
        cy.get('.prioritySelect').select('Low');


        cy.get('#addButton').click();

        cy.get('ul li')
            .contains(Text) 
            .parents('li')    
            .find('.deadlineTask') 
            .should('have.text', '2025-05-25'); 
        
        cy.get('ul li')
            .contains(Text) 
            .parents('li')    
            .find('.priorityTask') 
            .should('have.text', 'Low'); 
    });
})

describe('Удаление задачи', () => {
    beforeEach(() => {
        cy.visit('/'); 
    });
    afterEach(() => {
        cy.wait(1000);
    });

    it('Удаление задачи', () => {
        cy.get('.addTask').click();
        const todoText = Math.random().toString(36).substring(2, 6);
        cy.get('#taskInput').type(todoText);
        cy.get('#addButton').click();

        cy.get('ul li')
            .contains(todoText) 
            .parents('li')    
            .find('.btn-action-cross') 
            .click();
        cy.get('ul li').contains(todoText).should('not.exist');
    });

})

describe('Проверка сортировок', () => {
    beforeEach(() => {
        cy.visit('/'); 
    });
    afterEach(() => {
        cy.wait(1000);
    });

    it('По срочности', () => {
        cy.get('.sort').select('по срочности');
        cy.get('.sort_button').click()

        cy.get('.list-item') 
            .then(($taskItems) => {

                const deadlines = [];


                $taskItems.each((index, el) => {
                    const deadlineText = Cypress.$(el).find('.info div:last-child').text();  

                    const trimmedDeadline = deadlineText.trim();
                    deadlines.push(trimmedDeadline);
                });

                cy.wrap(deadlines).then((originalDeadlines) => {
                    const expectedOrder = ['2025-05-25', '2025-05-29', '2025-05-30', 'Дедлайн не задан', '2025-05-27'];

                    expect(originalDeadlines).to.deep.equal(expectedOrder);
                });
        });
    })

    it('По приоритету', () => {
        cy.get('.sort').select('по приоритету');
        cy.get('.sort_button').click()

        cy.get('.list-item') 
            .then(($taskItems) => {

                const priorities = [];


                $taskItems.each((index, el) => {
                    const PText = Cypress.$(el).find('.info div:first-child').text();  

                    const trimmedP = PText.trim();
                    priorities.push(trimmedP);
                });

                cy.wrap(priorities).then((originalP) => {
                    const expectedOrder = ['Critical', 'High', 'Medium', 'Low', 'Medium'];

                    expect(originalP).to.deep.equal(expectedOrder);
                });
        });
    

    })

})







