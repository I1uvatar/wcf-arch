@echo off
set usr=domain_or_computername\username


@echo *****************************************************************************
@echo                       Registering
@echo *****************************************************************************
@echo on	

netsh http add urlacl url=http://+:8733/BookService user=%usr%


