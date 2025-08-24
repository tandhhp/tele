import { apiProvinceList } from "@/services/settings/province";
import { DeleteOutlined, MoreOutlined, SettingOutlined } from "@ant-design/icons";
import { PageContainer, ProTable } from "@ant-design/pro-components"
import { history } from "@umijs/max";
import { Button, Dropdown, Popconfirm } from "antd";

const Index: React.FC = () => {
    return (
        <PageContainer>
            <ProTable
                search={{
                    layout: 'vertical'
                }}
                columns={[
                    {
                        title: '#',
                        valueType: 'indexBorder',
                        width: 30,
                        align: 'center'
                    },
                    {
                        title: 'Tỉnh/Thành phố',
                        dataIndex: 'name'
                    },
                    {
                        title: 'Xã/Phường',
                        dataIndex: 'districtCount',
                        valueType: 'digit',
                        width: 120,
                        search: false
                    },
                    {
                        title: 'Tác vụ',
                        valueType: 'option',
                        render: (_, record) => [
                            <Dropdown key={`more`} menu={{
                                items: [
                                    {
                                        key: 'district',
                                        label: 'Quản lý xã/phường',
                                        icon: <SettingOutlined />,
                                        onClick: () => {
                                            history.push(`/settings/province/district/${record.id}`);
                                        }
                                    }
                                ]
                            }}>
                                <Button type="dashed" icon={<MoreOutlined />} size="small" />
                            </Dropdown>,
                            <Popconfirm key={`delete`} title="Bạn có chắc chắn muốn xóa tỉnh/thành phố này?" onConfirm={async () => {
                                // Handle delete confirmation
                            }}>
                                <Button type="primary" icon={<DeleteOutlined />} danger size="small" />
                            </Popconfirm>
                        ],
                        width: 60
                    }
                ]}
                request={apiProvinceList}
            />
        </PageContainer>
    )
}

export default Index;