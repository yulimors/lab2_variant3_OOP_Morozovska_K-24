<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<html>
			<head>
				<meta charset="utf-8"/>
				<style>
					body { font-family: Arial, sans-serif; padding: 20px; }
					h2 { color: #0056b3; }
					table { border-collapse: collapse; width: 100%; margin-top: 20px; }
					th, td { border: 1px solid #ddd; padding: 12px; text-align: left; }
					th { background-color: #0056b3; color: white; }
					tr:nth-child(even){background-color: #f2f2f2;}
				</style>
			</head>
			<body>
				<h2>Студенти гуртожитку №16</h2>
				<table>
					<tr>
						<th>П.І.Б.</th>
						<th>Факультет</th>
						<th>Кафедра</th>
						<th>Курс</th>
						<th>Кімната</th>
						<th>Телефон</th>
					</tr>
					<xsl:for-each select="Dormitory/Student">
						<tr>
							<td>
								<xsl:value-of select="@Name"/>
							</td>
							<td>
								<xsl:value-of select="@Faculty"/>
							</td>
							<td>
								<xsl:value-of select="@Department"/>
							</td>
							<td>
								<xsl:value-of select="@Course"/>
							</td>
							<td>
								<xsl:value-of select="@Room"/>
							</td>
							<td>
								<xsl:value-of select="@Phone"/>
							</td>
						</tr>
					</xsl:for-each>
				</table>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>