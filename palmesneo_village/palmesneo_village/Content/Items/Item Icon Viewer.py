import pygame
import sys
import os
from PIL import Image

# Инициализация Pygame
pygame.init()

# Параметры
ICON_SIZE = 24
GRID_WIDTH = 16  # 256 / 16 = 16 иконок в ширину
GRID_HEIGHT = 16  # 256 / 16 = 16 иконок в высоту
DISPLAY_SCALE = 2  # Масштаб отображения для лучшей видимости
WINDOW_WIDTH = GRID_WIDTH * ICON_SIZE * DISPLAY_SCALE
WINDOW_HEIGHT = GRID_HEIGHT * ICON_SIZE * DISPLAY_SCALE

# Создание окна
screen = pygame.display.set_mode((WINDOW_WIDTH, WINDOW_HEIGHT))
pygame.display.set_caption("Индексы иконок")

# Загрузка изображения
def load_sprite_sheet():
    # Получаем путь к директории, в которой находится скрипт
    script_dir = os.path.dirname(os.path.abspath(__file__))
    # Формируем путь к файлу items_tileset.png
    image_path = os.path.join(script_dir, "items_icons.png")
    
    try:
        print(f"Попытка загрузить изображение из: {image_path}")
        image = Image.open(image_path)
        print("Изображение успешно загружено")
        return image
    except Exception as e:
        print(f"Ошибка при загрузке изображения: {e}")
        sys.exit(1)

# Отрисовка индексов иконок
def draw_indexes(image):
    # Преобразование изображения в формат Pygame
    mode = image.mode
    size = image.size
    data = image.tobytes()
    
    py_image = pygame.image.fromstring(data, size, mode)
    
    # Масштабирование изображения
    py_image = pygame.transform.scale(py_image, (WINDOW_WIDTH, WINDOW_HEIGHT))
    
    # Отрисовка изображения
    screen.blit(py_image, (0, 0))
    
    # Отрисовка индексов
    font = pygame.font.SysFont('Arial', 10 * DISPLAY_SCALE)
    
    for y in range(GRID_HEIGHT):
        for x in range(GRID_WIDTH):
            index = y * GRID_WIDTH + x
            text = font.render(str(index), True, (255, 0, 0))
            rect = text.get_rect(center=(
                x * ICON_SIZE * DISPLAY_SCALE + ICON_SIZE * DISPLAY_SCALE // 2,
                y * ICON_SIZE * DISPLAY_SCALE + ICON_SIZE * DISPLAY_SCALE // 2
            ))
            screen.blit(text, rect)

def main():
    # Загрузка изображения
    image = load_sprite_sheet()
    
    # Основной цикл
    running = True
    while running:
        for event in pygame.event.get():
            if event.type == pygame.QUIT:
                running = False
        
        # Отрисовка
        screen.fill((0, 0, 0))
        draw_indexes(image)
        pygame.display.flip()
    
    pygame.quit()
    sys.exit()

if __name__ == "__main__":
    main()