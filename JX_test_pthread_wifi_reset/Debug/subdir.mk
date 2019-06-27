################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
CPP_SRCS += \
../TCP_Client.cpp \
../Uart0.cpp \
../XJ.cpp \
../fa_cam202.cpp \
../gpio.cpp \
../main.cpp \
../pcm.cpp \
../test.cpp \
../wifi_server.cpp 

C_SRCS += \
../wifi_confl.c 

OBJS += \
./TCP_Client.o \
./Uart0.o \
./XJ.o \
./fa_cam202.o \
./gpio.o \
./main.o \
./pcm.o \
./test.o \
./wifi_confl.o \
./wifi_server.o 

CPP_DEPS += \
./TCP_Client.d \
./Uart0.d \
./XJ.d \
./fa_cam202.d \
./gpio.d \
./main.d \
./pcm.d \
./test.d \
./wifi_server.d 

C_DEPS += \
./wifi_confl.d 


# Each subdirectory must supply rules for building sources it contributes
%.o: ../%.cpp
	@echo 'Building file: $<'
	@echo 'Invoking: GCC C++ Compiler'
	g++ -O0 -g3 -Wall -c -fmessage-length=0 -MMD -MP -MF"$(@:%.o=%.d)" -MT"$(@:%.o=%.d)" -o"$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '

%.o: ../%.c
	@echo 'Building file: $<'
	@echo 'Invoking: GCC C Compiler'
	gcc -O0 -g3 -Wall -c -fmessage-length=0 -MMD -MP -MF"$(@:%.o=%.d)" -MT"$(@:%.o=%.d)" -o"$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


