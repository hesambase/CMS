import uuid from 'uuid-js'

describe('Register page', () => {
    it('should display error messages on invalid form submit', () => {
        // Arrange
        cy.visit('/');
        cy.contains('Sign Up').click();

        // Act
        cy.get('[type=submit]').click();

        // Assert
        cy.get('.error-messages');
        cy.contains('Username is required');
        cy.contains('Email is required');
        cy.contains('Password is required');
    });

    it('should remove username error message when valid data is entered', () => {
        // Arrange
        cy.get('.error-messages');
        cy.contains('Username is required');

        // Act
        cy.get('#register-username-field')
            .type('test')
            .blur();

        // Assert
        cy.get('.error-messages');
        cy.contains('Username is required').should('not.exist');
    });

    it('should remove email error message when valid data is entered', () => {
        // Arrange
        cy.get('.error-messages');
        cy.contains('Email is required');

        // Act
        cy.get('#register-email-field').type('test@gmail.com').blur();

        // Assert
        cy.get('.error-messages');
        cy.contains('Email is required').should('not.exist');
    });

    it('should remove password error message when valid data is entered', () => {
        // Arrange
        cy.get('.error-messages');
        cy.contains('Password is required');

        // Act
        cy.get('#register-password-field').type('test12345').blur();

        // Assert
        cy.get('.error-messages').should('not.have.html');
        cy.get('Password is required').should('not.exist');
    });

    it('should display API errors on validation exceptions', () => {
        // Arrange
        cy.get('.error-messages').should('not.have.html');

        // Act
        cy.get('[type=submit]').click();

        // Assert
        cy.get('.error-messages');
        cy.contains('email has already been taken');
        cy.contains('username has already been taken');
    });

    it('should proceed to register and redirect the user on a valid sign up', () => {
        // Arrange, generate random UUIDs for username and email
        const randomUsername = uuid.create(4)
            .toString()
            .substring(0, 15);   

        // Act
        cy.get('#register-username-field').clear().type(randomUsername);
        cy.get('#register-email-field').clear().type(`${randomUsername}@gmail.com`);
        cy.get('#register-password-field').clear().type('test12345');
        cy.get('[type=submit]').click();

        // Assert, wait for redirect to finish
        cy.wait(1500);
        cy.get('.logo-font');
        cy.contains('A place to share your knowledge.');
    });
});
