/*
 * fan_pwm.h
 *
 *  Created on: Jan 25, 2021
 *      Author: HUBI
 */

#ifndef INC_FAN_PWM_H_
#define INC_FAN_PWM_H_

/* Config --------------------------------------------------------------------*/

/* Includes ------------------------------------------------------------------*/
#include "stm32f7xx_hal.h"

/* Typedef -------------------------------------------------------------------*/
#define FAN_PWM_TimerType   TIM_HandleTypeDef*
#define FAN_PWM_ChannelType uint16_t

typedef struct {
  FAN_PWM_TimerType Timer;
  FAN_PWM_ChannelType Channel;
  float Duty;
} FAN_PWM_HandleTypeDef;

/* Define --------------------------------------------------------------------*/

/* Macro ---------------------------------------------------------------------*/

/* Public variables ----------------------------------------------------------*/

/* Public function prototypes ------------------------------------------------*/

/**
 * @brief PWM-controlled fan initialization.
 * @param[in] fanpwm fan handler
 * @return None
 */
void FAN_PWM_Init(FAN_PWM_HandleTypeDef* fanpwm);

/**
 * @brief Sets duty of PWM-controlled fan.
 * @param[in] fanpwm fan handler
 * @param[in] duty       PWM duty in percents
 * @return None
 */
void FAN_PWM_SetDuty(FAN_PWM_HandleTypeDef* fanpwm, float duty);

#endif /* INC_FAN_PWM_H_ */
