# -*- mode: ruby -*-
# vi: set ft=ruby :

Vagrant.configure("2") do |config|
  config.vm.box = "bento/ubuntu-16.04"

  config.vm.provision "shell", path: "bootstrap.sh"
  config.vm.provision "shell", inline: "cd /vagrant && sudo docker-compose up -d", run: "always"
  config.vm.provision "shell", inline: "cd /vagrant && sudo docker-compose ps", run: "always"

  # for api
  config.vm.network "forwarded_port", guest: 80, host: 7080
  
  # for ssh
  config.vm.network "forwarded_port", id: "ssh", guest: 22, host: 9022
  
  config.vm.provider "virtualbox" do |vb|
    # Display the VirtualBox GUI when booting the machine
    vb.gui = false

    # Customize the amount of memory on the VM
    vb.memory = "4096"
    vb.cpus = "2"
  end

end