#!groovy

String GIT_COMMIT
String GIT_BRANCH

node('docker') {
  def STAGING_SERVER_ADDRESS = 'YOUR STAGING SERVER ADDRESS'
  def WORKSPACE = pwd()
  def webImage
  def dbImage

  stage('Checkout') {
    // Clear out the workspace ahead of time
    // This prevents issues where the workspace contains old files
    sh('sudo rm -rf *')

    // Checkout the commit referenced by the build
    checkout(scm)

    // Get the commit and store it in a variable
    sh('git rev-parse HEAD > GIT_COMMIT')
    GIT_COMMIT = readFile('GIT_COMMIT').trim()

    // Get the branch and store it in a variable
    sh('git rev-parse --abbrev-ref HEAD > GIT_BRANCH')
    GIT_BRANCH = readFile('GIT_BRANCH').trim()

    if (GIT_BRANCH == 'HEAD') {
      // Check if env.BRANCH_NAME is set
      if (env.BRANCH_NAME != null) {
        GIT_BRANCH = env.BRANCH_NAME
      }
    }

    echo("Continuing build on branch ${GIT_BRANCH}")
  }

  stage('Clean up') {
    // Clean up all unused volumes.
    sh("docker volume ls -q --filter dangling=true | xargs --no-run-if-empty docker volume rm || exit 0")

    // This removes any containers that exited but weren't cleaned up
    sh("docker ps -a --filter 'status=exited' -q --no-trunc | xargs --no-run-if-empty docker rm -v || exit 0")

    // This removes images that are more than one hour old, so it's likely that they are no longer being used
    sh("docker images | grep -v -E 'minute|second' | grep -E '^cre8tiv/cre8tiv-deploydemo-web|^<none' | awk '{print \$3}' | xargs --no-run-if-empty docker rmi -f || exit 0")
  }

  stage('Build test images') {
    // Build web test server
    testImage = docker.build("cre8tiv/cre8tiv-deploydemo-test:${GIT_COMMIT}", "-f ./docker/Dockerfile.web.test .")
  }

  stage('Run tests') {
    testImage.inside() {
      try {

        sh("dotnet test \"./DemoProject.Tests/DemoProject.Tests.csproj\" -v d -c Release")

        currentBuild.result = 'SUCCESS'
      } catch (Exception) {
        currentBuild.result = 'FAILURE'

        echo('Detected build failures, setting build result to FAILURE')
      }
    }
  }

  if (GIT_BRANCH == 'develop' || GIT_BRANCH == 'master') {

    if (currentBuild.result == 'SUCCESS') {
      stage('Build deployment images') {
        // Build the web image
        // Release on master, debug on staging and other branches
        if (GIT_BRANCH == 'master') {
          webImage = docker.build("cre8tiv/cre8tiv-deploydemo-web:${GIT_COMMIT}", "-f ./docker/Dockerfile.web.prod .")
          //leaving this to facilitate future testing to deploy to staging
          //webImage = docker.build("cre8tiv/cre8tiv-deploydemo-web:develop", "-f ./docker/Dockerfile.web.prod .")
        } else {
          webImage = docker.build("cre8tiv/cre8tiv-deploydemo-web:${GIT_COMMIT}", "-f ./docker/Dockerfile.web.staging .")
          //leaving this to facilitate future testing to deploy to staging
          //webImage = docker.build("cre8tiv/cre8tiv-deploydemo-web:develop", "-f ./docker/Dockerfile.web.staging .")
        }
      }

      stage('Push images') {
        // Push images to Docker Hub
        try {
          // Build nginx image.
          nginxImage = docker.build("cre8tiv/cre8tiv-deploydemo-nginx:${GIT_COMMIT}", "-f ./docker/Dockerfile.nginx .")
          //leaving this to facilitate future testing to deploy to staging
          //nginxImage = docker.build("cre8tiv/cre8tiv-deploydemo-nginx:develop", "-f ./docker/Dockerfile.nginx .")

          // The first parameter is the uri of the registry.
          // It defaults to DockerHub if left blank.
          docker.withRegistry('', 'docker-hub-awesomebot') {
            // Push image tagged with the current branch name
            webImage.push(GIT_COMMIT)
            webImage.push(GIT_BRANCH)
            //leaving this to facilitate future testing to deploy to staging
            //webImage.push("develop")

            nginxImage.push(GIT_COMMIT)
            nginxImage.push(GIT_BRANCH)
            //leaving this to facilitate future testing to deploy to staging
            //nginxImage.push("develop")
          }
        } catch (Exception) {
          currentBuild.result = 'UNSTABLE'

          echo('Failed to push images to registry, setting build result to UNSTABLE')
        }
      }

      if (GIT_BRANCH == 'develop') {

        stage('Deploy to Staging') {
          // SSH into the azure virtual machine using secure Jenkins credentials
          sshagent(credentials: ['azure-staging']) {
            // Copy compose file for staging.
            sh("scp -o StrictHostKeyChecking=no docker-compose-staging.yml azureuser@${STAGING_SERVER_ADDRESS}:docker-compose.yml")

            // Ensure docker folder exists to hold bot token.
            sh("ssh -o StrictHostKeyChecking=no -l azureuser ${STAGING_SERVER_ADDRESS} 'mkdir -p ~/.docker/'")

            // Copy docker config for bot token.
            sh("scp -o StrictHostKeyChecking=no ./docker/config.json azureuser@${STAGING_SERVER_ADDRESS}:~/.docker/config.json")

            // Run the server using compose.
            def composeCommands = "sudo docker-compose down && sudo docker-compose pull && sudo docker-compose up -d"
            sh("ssh -o StrictHostKeyChecking=no -l azureuser ${STAGING_SERVER_ADDRESS} '${composeCommands}'")
          }
        }
      } else if (GIT_BRANCH == 'master') {
        //TODO
      }

    } else {
      echo("Image will not be pushed due to build status: ${currentBuild.result}.")
    }
  }
}