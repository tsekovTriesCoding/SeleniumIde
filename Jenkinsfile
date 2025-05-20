pipeline {
    agent any

    stages {
         stage('Restore dependencies') {
            steps {
                bat 'dotnet restore'
            }
        }

        stage('Build project') {
            steps {
                bat 'dotnet build --configuration Release'
            }
        }

        stage('Run dotnet tests') {
            steps {
                bat 'dotnet test'
            }
        }  
    }
}