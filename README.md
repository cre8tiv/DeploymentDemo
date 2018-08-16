# DeploymentDemo
The goal of this demonstration is to show how to setup and run a CI pipeline using Jenkins.  In general this tutorial will walk through the steps required to setup and execute this pipeline.  When all of the configuration steps have been completed you will be able to:

1. Commit a change to a repository on GitHub
2. The change will trigger Jenkins running in Azure to perform a build
3. Tests in the project will be run by the build pipeline, and upon success will
4. Create Docker images that are pushed to Docker Hub and will
5. Push the deployment of the application to Azure Kubernetes 

## Prerequisites

To configure the entire process you will require:

1. An Azure account.  If you do not have one, you can sign up for one for free here: https://azure.microsoft.com/en-us/free/
2. A GitHub account.  If you do not have one, you can sign up here:  https://github.com/join
3. A Docker Hub account.  If you do not have one you can sign up here:  https://hub.docker.com/

Let's start with the steps required to create the environments required for Jenkins and for your deployment of the application.  Sign into Azure using your account.

### Jenkins
Instructions for deploying Jenkins in Azure that can help to get you started may be found here:  
https://github.com/Azure/azure-quickstart-templates/tree/master/101-jenkins

Follow these instructions and once you have successfully logged into Jekins, perform these steps:

1. Install suggested plugins
2. Create the first Admin user

### Docker Hub
You need to have a Docker hub account.  Create one here:  https://hub.docker.com

Once you have created an account, you'll need to create some repositories for the docker images that you are going to create through the deployment process.  You'll need 3:

* web
* test
* nginx

### Deployment Environment
You need to have an environment to deploy the application against in Azure that supports Docker containers.  For this demonstration, we will use AKS (Azure Kubernetes Service).  The easiest way to set this up is to use the wizard through Azure to create Kubernetes Service under your subscription.  It is recommended that production clusters have a minimum of 3 nodes, but since this is going to be used for demonstration purposes, you can specify only having one node.  

Again, you can reduce the node size since this is for demonstration purposes to use a B2s or similiar for lower resource cost.

You can accept all of the defaults that AKS provides in walking you through the service.  A thorough walk-through of setting up and running a AKS instance can be found here:  https://docs.microsoft.com/en-us/azure/aks/kubernetes-walkthrough-portal

1. From the CloudShell prompt, set the subscription you want to use, i.e.: az account set --subscription "Visual Studio Professional"
2. Deploy a Kubernetes cluster: az aks create --resource-group <your resource group name> --name myAKSCluster --node-count 1 --enable-addons monitoring --generate-ssh-keys


### Connecting Jenkins & GitHub

You'll need to allow Jenkins to be able to connect to GitHub to work with your repository.  You'll do this by generating a personal access token to use with Jenkins.

GitHub
1. Settings > Developer Settings > Personal access tokens > Generate new token
2. Provide Token description, something like "My Jenkins Server"
3. Select scopes > repo, which will allow Jenkins to interact with your repos
4. Copy the personal access token shown as once you navigate away from this screen you won't see the token again and will have to re-generate one if you don't have this text.

Jenkins
1. Jenkins > New Item
2. Enter a name for this workflow, something like "Deployment Demo" and choose Multibranch Pipeline, then Ok.

General
1. General > Enter Name & Display name

Branch Sources
1. Branch Sources > Add source > GitHub
2. Click Add button under Credentials > Jenkins (this will store the credentials and make them globally available to other jobs)
3. Leave Domain as Global credentials
4. Kind > Username with password
5. Scope > Leave as Global
6. Username > is the GitHub user ID
7. Password > Paste in the personal access token from GitHub
8. Once created, select the newly created credentials.  If they are correct you'll receive no errors that they are invalid credentials.
9. Owner > specify your GitHub owner account
10. Repository > this repository or the repository you have cloned
11. Discover branches > Strategy > All branches
12. Discover pull requests from origin > Strategy > Merging the pull request with the current target branch revision
13. Close out or delete the Discover pull requests from forks
14. Leave Property strategy alone

Build Configuration
1. Mode > by Jenkinsfile
2. Script Path > Jenkinsfile

Scan Multibranch Pipeline Triggers
1. Check Periodically if not otherwise run
2. Set the Interval to 15 minutes


You have now configured a job in Jenkins.  Let's test by creating a PR in GitHub...

Since Jenkins is set to build on a pull request, we'll create a develop branch to trigger a build.

If you've created a branch, make some change and then commit it.

Create a pull request on GitHub.