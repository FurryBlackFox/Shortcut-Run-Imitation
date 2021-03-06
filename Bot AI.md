# Bot AI

> :warning: Всё, что написано ниже может измениться и быть переделано с нуля в любой момент.

Если обощить, то искусственный интеллект бота заключается в двух вещах:
- Система движения по контрольным точкам
- Система реагирования на препятствия

## Система контрольных точек

За основу была взята система контрольных точек с YouTube канала Game Dev Guide [Tutorial Link](https://www.youtube.com/watch?v=MXCZ-n5VyJc)

### Создание простого маршрута

Суть максимально проста: каждая контрольная точка содержит ссылку на следующую контрольную точку.
Это позволяет вручную построить маршрут  для искусственного интеллекта от старта до финиша.
Если добавить группы контрольных точек в префабы дороги, то можно сильно упросить процесс создания маршрута.

<img src="https://github.com/FurryBlackFox/Shortcut-Run-Imitation/blob/main/Description%20Data/Shortcut%20Run%20Bot%20AI%20Waypoints%20In%20Road%20Prefab.jpg" width="710" height="500">

Продолжая автоматизацию, с помощью скрипта можно соединить все соседние контрольные точки.
Таким образом, мы можем, без лишних усилий, с помощью одной кнопки, выполнить создание простого маршрута, не имеющего ветвей и срезок.

<img src="https://github.com/FurryBlackFox/Shortcut-Run-Imitation/blob/main/Description%20Data/Shortcut%20Run%20Bot%20AI%20Simple%20Waypoints%20Route.jpg" width="747" height="649">

### Добавление ветвей

Согласно описанию кор геймлпея, боты ничем не отличаются от игрока.
Следовательно, им тоже необходимо как-то срезать маршрут.
Для этого введем систему ветвления - теперь каждая контрольная точка может содержать еще дополнительные ссылки на другие контрольные точки.

<img src="https://github.com/FurryBlackFox/Shortcut-Run-Imitation/blob/main/Description%20Data/Shortcut%20Run%20Bot%20AI%20Simple%20Branches.jpg" width="801" height="665">

Для корректной работы введем:
- Стоимость движения по альтернативному маршуту, т.к. передвижение по воде расходует планки, имеющиется у персонажа
- Контролирующую функцию, позволяющую задавать частоту перехода к альтернативному маршруту
- Выбор случайного дополнительного маршрута

<img src="https://github.com/FurryBlackFox/Shortcut-Run-Imitation/blob/main/Description%20Data/Shortcut%20Run%20Bot%20AI%20More%20Complex%20Branches.jpg" width="814" height="623">

Так же система с ответвлениями позволяет создавать вот такие развилки

<img src="https://github.com/FurryBlackFox/Shortcut-Run-Imitation/blob/main/Description%20Data/Shortcut%20Run%20Bot%20AI%20Branches%20Fork.jpg" width="748" height="617">

Дело за автоматизацией.
- Для автоматического создания ветвей понадобится знать:
  - Расстояние до финиша по маршруту
  - Расстояние между проверяемыми контрольными точками
  - Пороговый коэффициент, контролирующий минимальную выгоду за срезку маршрута по воде
- Для автоматического расчета стоимости маршрута по воде понадобится знать только расстояние между контрольными точками и средний расход планок на метр пути.

Ниже представлен автоматически сгенерированный маршрут с коэффициентом эффективности срезок 1.5.

<img src="https://github.com/FurryBlackFox/Shortcut-Run-Imitation/blob/main/Description%20Data/Shortcut%20Run%20Bot%20AI%20Generated%20Branches.jpg">

> Продолжение следует

## Система движения по контрольным точкам

## Система реагирования на препятствия
