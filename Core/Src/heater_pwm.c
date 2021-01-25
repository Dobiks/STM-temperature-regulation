/*
 * heater_pwm.c
 *
 *  Created on: Jan 25, 2021
 *      Author: HB
 */


/* Includes ------------------------------------------------------------------*/
#include "heater_pwm.h"

/* Typedef -------------------------------------------------------------------*/

/* Define --------------------------------------------------------------------*/

/* Macro ---------------------------------------------------------------------*/

/* Private variables ---------------------------------------------------------*/

/* Public variables ----------------------------------------------------------*/

/* Private function prototypes -----------------------------------------------*/

/* Private function ----------------------------------------------------------*/

/* Public function -----------------------------------------------------------*/

/**
 * @brief PWM-controlled heater initialization.
 * @param[in] hheaterpwm Heater handler
 * @return None
 */
void HEATER_PWM_Init(HEATER_PWM_HandleTypeDef* heaterpwm)
{
  HAL_TIM_PWM_Start(heaterpwm->Timer, heaterpwm->Channel);
}

/**
 * @brief Sets duty of PWM-controlled heater.
 * @param[in] hheaterpwm Heater handler
 * @param[in] duty       PWM duty in percents
 * @return None
 */
void HEATER_PWM_SetDuty(HEATER_PWM_HandleTypeDef* heaterpwm, float duty)
{
  heaterpwm->Duty = duty;
  int COMPARE = (duty * 10);
  __HAL_TIM_SET_COMPARE(heaterpwm->Timer, heaterpwm->Channel, COMPARE);
}
