/*
 * fan_pwm.c
 *
 *  Created on: Jan 25, 2021
 *      Author: HUBI
 */


/* Includes ------------------------------------------------------------------*/
#include "fan_pwm.h"

/* Typedef -------------------------------------------------------------------*/

/* Define --------------------------------------------------------------------*/

/* Macro ---------------------------------------------------------------------*/

/* Private variables ---------------------------------------------------------*/

/* Public variables ----------------------------------------------------------*/

/* Private function prototypes -----------------------------------------------*/

/* Private function ----------------------------------------------------------*/

/* Public function -----------------------------------------------------------*/

/**
 * @brief PWM-controlled fan initialization.
 * @param[in] fanpwm Fan handler
 * @return None
 */
void FAN_PWM_Init(FAN_PWM_HandleTypeDef* fanpwm)
{
  HAL_TIM_PWM_Start(fanpwm->Timer, fanpwm->Channel);
}

/**
 * @brief Sets duty of PWM-controlled fan.
 * @param[in] fanpwm Fan handler
 * @param[in] duty       PWM duty in percents
 * @return None
 */
void FAN_PWM_SetDuty(FAN_PWM_HandleTypeDef* fanpwm, float duty)
{
  fanpwm->Duty = duty;
  int COMPARE = (duty * 10);
  __HAL_TIM_SET_COMPARE(fanpwm->Timer, fanpwm->Channel, COMPARE);
}
