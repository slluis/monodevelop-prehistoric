#!/usr/bin/perl -w

use strict;

my $file;
my $stockid;
my $stockprop;
my $type;

my %properties;
my %stocks;

open IN, "/bin/ls |" || die "Can't spawn ls process";

while (<IN>)
{
    chop $_;
    $file = $_;

    $type = `file $file`;
    next unless ($type =~ /PNG image data/);

    $type =~ /.+:\s+PNG image data, ([0-9]+) x ([0-9]+)/;
    my ($w, $h) = ($1, $2);

    next if ($w > 48 || $h > 48);

    $stockid = $file;

    # remove noise
    $stockid =~ s/[.]//g;
    $stockid =~ s/Icons//g;
    $stockid =~ s/16x16//g;
    $stockid =~ s/32x32//g;

    # titlecase acronyms
    $stockid =~ s/VB/Vb/g;
    $stockid =~ s/JScript/Jscript/g;
    $stockid =~ s/XML/Xml/g;
    $stockid =~ s/HTML/Html/g;
    $stockid =~ s/UML/Html/g;
    $stockid =~ s/UML/Html/g;
    $stockid =~ s/DOS/Dos/g;
    $stockid =~ s/ASP/Asp/g;
    $stockid =~ s/ILD/Ild/g;

    # ucfirst some known whole words
    if ($stockid =~ /^(NETWORK|CDROM|FLOPPY|DRIVE)$/) {
	$stockid = ucfirst lc $stockid;
    }

    # get the property name here
    $stockprop = $stockid;

    # insert dashes to separate words and lowercase everything
    $stockid =~ s/([A-Z])/-$1/g;
    $stockid =~ s/^-//g;
    $stockid =~ tr/A-Z/a-z/;

    $stockid = "md-$stockid";

    # replace invalid characters in the property name
    $stockprop =~ s/\#/Sharp/g;
    $stockprop =~ s/\+\+/PlusPlus/g;

    $properties {$stockprop} = $stockid;
    push @{$stocks {$stockid}}, $file, $w;
};

print "\t\tstatic void SetupIconFactory ()\n\t\t{\n";

foreach $stockid (sort keys %stocks) {
    my @alt = @{$stocks {$stockid}};

    if ($#alt == 1) {
	$file = $alt[0];
	print "\t\t\tAddToIconFactory (\"$stockid\", \"$file\");\n";
    } else {
	while ($file = shift @alt) {
	    my $w = shift @alt;
	    my $size;
	    if ($w <= 16) { $size = "Gtk.IconSize.Menu"; }
	    elsif ($w <= 18) { $size = "Gtk.IconSize.SmallToolbar"; }
	    elsif ($w <= 20) { $size = "Gtk.IconSize.Button"; }
	    elsif ($w <= 24) { $size = "Gtk.IconSize.LargeToolbar"; }
	    elsif ($w <= 32) { $size = "Gtk.IconSize.Dnd"; }
	    else { $size = "Gtk.IconSize.Dialog"; }

	    print "\t\t\tAddToIconFactory (\"$stockid\", \"$file\", $size);\n";
	};
    };
};

print "\t\t}\n\n";

foreach $stockprop (sort keys %properties) {
    $stockid = $properties {$stockprop};

    print "\t\tpublic static string $stockprop { get { return \"$stockid\"; } }\n";
};

