#include <stdio.h>
#include <stdarg.h>
#include <string.h>

typedef void (*log_callback_t)(int level, const char* message);

static log_callback_t g_log_callback = NULL;

void set_log_callback(log_callback_t callback) {
    g_log_callback = callback;
}

void retro_log_printf(int level, const char* fmt, ...) {
    if (g_log_callback) {
        va_list args;
        va_start(args, fmt);
        char buffer[4096];
        vsnprintf(buffer, sizeof(buffer), fmt, args);
        va_end(args);
        g_log_callback(level, buffer);
    }
}