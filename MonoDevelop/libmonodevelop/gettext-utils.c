/*
 * Copyright (C) 2004 Jorn Baayen <jorn@nl.linux.org>
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License as
 * published by the Free Software Foundation; either version 2 of the
 * License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * General Public License for more details.
 *
 * You should have received a copy of the GNU General Public
 * License along with this program; if not, write to the
 * Free Software Foundation, Inc., 59 Temple Place - Suite 330,
 * Boston, MA 02111-1307, USA.
 */

#ifdef HAVE_CONFIG_H
#include "config.h"
#endif

#include <libintl.h>

#include "gettext-utils.h"

void
intl_init (const char *package)
{
#ifdef ENABLE_NLS
	bindtextdomain (package, GNOMELOCALEDIR);
	bind_textdomain_codeset (package, "UTF-8");
	textdomain (package);
#endif
}

const char *
intl_get_string (const char *string)
{
#ifdef ENABLE_NLS
	return gettext (string);
#else
	return string;
#endif
}

const char *
intl_get_plural_string (const char *singular,
			const char *plural,
			long n)
{
#ifdef ENABLE_NLS
	return ngettext (singular, plural, n);
#else
	if (n == 1)
		return singular;
	else
		return plural;
#endif
}
