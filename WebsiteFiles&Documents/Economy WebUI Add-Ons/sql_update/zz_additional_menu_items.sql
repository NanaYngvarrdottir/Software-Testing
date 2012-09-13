
INSERT INTO `wi_pagemanager` (`id`, `rank`, `active`, `url`, `target`, `display`) VALUES
('webui_menu_item_getregion', 1, '1', 'index.php?page=getland', '_self', '1');


INSERT INTO `wi_pagemanager` (`id`, `rank`, `active`, `url`, `target`, `display`, `parent`) VALUES
('webui_menu_item_getcurrency', 10, '1', 'index.php?page=getcurrency', '_self', '1', 'webui_menu_item_account'),
('webui_menu_item_getcurrencyhistory', 11, '1', 'index.php?page=getcurrencyhistory', '_self', '1', 'webui_menu_item_account'),
('webui_menu_item_currencytransfer', 12, '1', 'index.php?page=transfercurrency', '_self', '1', 'webui_menu_item_account');


