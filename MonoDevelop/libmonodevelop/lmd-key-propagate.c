/*
 *
 * Workaround for the inability to propagate keypresses
 *
 */

#include <gtk/gtk.h>
#include <gtk/gtkwidget.h>
#include <gdk/gdkevents.h>

void
lmd_propagate_eventkey (GtkWidget *widget, GdkEventKey *key)
{
	gtk_propagate_event (widget, (GdkEvent *)key);
}
