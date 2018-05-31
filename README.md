# DeploymentDemo
Demo project to teach using Jenkins for automated deployments to Azure.  This will deploy a ASP.NET Core MVC web application running in a Docker container with an NGINX proxy server to Azure resources.  We will build the application in the Jenkins pipeline, including running tests for the project, and upon successful build, push the Docker images to Docker Hub, and then trigger the deployment of the images to Azure.

## Prerequisites
### Jenkins
You need to have a Jenkins build environment.  One option is to configure one in Azure.  These instructions can get you started:  https://azuremarketplace.microsoft.com/en-us/marketplace/apps/azure-oss.jenkins?tab=Overview

The recommended configuration for virtual machin is a D2V2, but if you are just creating this resource to learn this tutorial, you can use a smaller VM instance such as a B2s.

Much of the following steps come from this source:  https://github.com/Azure/azure-quickstart-templates/tree/master/201-jenkins-acr

#### Connect to Jenkins with SSH port forwarding
By default the Jenkins instance is using the http protocol and listens on port 8080. Users shouldn't authenticate over unsecured protocols!

You need to setup port forwarding to view the Jenkins UI on your local machine. If you do not know the full DNS name of your instance, go to the Portal and find it in the deployment outputs here: Resource Groups > {Resource Group Name} > Deployments > {Deployment Name, usually 'Microsoft.Template'} > Outputs

Note that in the following steps, we are mapping the Jenkins UI to port 8080.  If you wish to use another port, then substitute that port number in the commands below.

##### If you are using Windows:
Install Putty or use any bash shell for Windows (if using a bash shell, follow the instructions for Linux or Mac).

Run this command:

```putty.exe -ssh -L 8080:localhost:8080 <User name>@<Public DNS name of instance you just created>```

Or follow these manual steps:

1. Launch Putty and navigate to 'Connection > SSH > Tunnels'
2. In the Options controlling SSH port forwarding window, enter 8080 for Source port. Then enter 127.0.0.1:8080 for the Destination. Click Add.
3. Click Open to establish the connection.

##### If you are using Linux or Mac:
Run this command:

```ssh -L 8080:localhost:8080 <User name>@<Public DNS name of instance you just created>```

Connecting

1. After you have started your tunnel, navigate to http://localhost:8080/ on your local machine.
2. Unlock the Jenkins dashboard for the first time with the initial admin password. To get this token, SSH into the VM and run sudo cat /var/lib/jenkins/secrets/initialAdminPassword
3. Your Jenkins instance is now ready to use! You can access a read-only view by going to http://< Public DNS name of instance you just created >.
4. Go ahead and install the common plugins that Jenkis recommends
5. Create a first admin user.  After you do so, you may find that the Setup Wizard completes, but shows a blank screen when you are done.  

4. Go to http://aka.ms/azjenkinsagents if you want to build/CI from this Jenkins master using Azure VM agents.

### Docker Hub
You need to have a Docker hub account.  Create one here:  https://hub.docker.com

Once you have created an account, you'll need to create some repositories for the docker images that you are going to create through the deployment process.  You'll need 3:

* web
* test
* nginx

### Azure VM or ACI
You need to have an environment to deploy the application against in Azure that supports Docker containers.
