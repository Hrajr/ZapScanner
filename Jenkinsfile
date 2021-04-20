pipeline {
    agent {
        docker {
            image any
            args '-v /root/.m2:/root/.m2'
            args '--network=docker-jenkins-sonarqube'
        }
    }
    stages {
        stage('Build and Test') {
            steps {
                sh 'dotnet build'
            }
        }
        stage('Sonar') {
            steps {
                sh "mvn sonar:sonar -Dsonar.host.url=http://host.docker.internal:9000/"
            }
        }
    }
}
