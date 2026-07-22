- in ExceptionHandlingMiddleware you are returning entire exception stack instead it should be logged and in return there should be some meaningfull message.

- remove request validation from controller as there is already custom validator

- resolve editorconfig error as we discussed

- there are some logic related issues, which i'm trying to undestand
