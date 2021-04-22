pipeline {
    agent {
        docker {
            image 'maven:3.6.2-jdk-11'
            args '-v /root/.m2:/root/.m2'
            args '--network=docker-jenkins-sonarqube'
        }
    }
    stages {
        stage('Build and Test') {
            steps {
                bat 'dotnet build'
            }
        }
        stage('Sonar') {
            steps {
                sh "mvn sonar:sonar -Dsonar.host.url=http://host.docker.internal:9000/"
            }
        }
    }
}
