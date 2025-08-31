import { apiTeamCreate, apiTeamDelete, apiTeamList, apiTeamUpdate } from "@/services/users/team";
import { DeleteOutlined, EditOutlined, LeftOutlined, MoreOutlined, PlusOutlined, UsergroupAddOutlined } from "@ant-design/icons";
import { ActionType, ModalForm, PageContainer, ProFormInstance, ProFormText, ProTable } from "@ant-design/pro-components"
import { history, useAccess, useParams } from "@umijs/max";
import { Button, Dropdown, message, Popconfirm } from "antd";
import { useEffect, useRef, useState } from "react";

const Index: React.FC = () => {
    const { id } = useParams<{ id: string }>();
    const access = useAccess();
    const [open, setOpen] = useState<boolean>(false);
    const actionRef = useRef<ActionType>();
    const formRef = useRef<ProFormInstance>();
    const [team, setTeam] = useState<any>();

    useEffect(() => {
        if (open && team) {
            formRef.current?.setFieldsValue(team);
        }
    }, [open, team]);

    const onFinish = async (values: any) => {
        values.departmentId = id;
        if (values.id) {
            await apiTeamUpdate(values);
        } else {
            await apiTeamCreate(values);
        }
        message.success('Thành công');
        actionRef.current?.reload();
        formRef.current?.resetFields();
        setTeam(undefined);
        return true;
    }

    return (
        <PageContainer extra={<Button icon={<LeftOutlined />} onClick={() => history.back()}>Quay lại</Button>}>
            <ProTable
                headerTitle={<Button type="primary" onClick={() => setOpen(true)} icon={<PlusOutlined />} hidden={!access.canAdmin}>Thêm nhóm</Button>}
                request={apiTeamList} params={{ departmentId: id }}
                actionRef={actionRef}
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
                        title: 'Tên nhóm',
                        dataIndex: 'name'
                    },
                    {
                        title: 'Thành viên',
                        valueType: 'digit',
                        search: false,
                        dataIndex: 'userCount'
                    },
                    {
                        title: 'Tác vụ',
                        valueType: 'option',
                        render: (_, record) => [
                            <Dropdown key={`action`} menu={{
                                items: [
                                    {
                                        key: 'edit',
                                        label: 'Chỉnh sửa',
                                        icon: <EditOutlined />,
                                        onClick: () => {
                                            setTeam(record);
                                            setOpen(true);
                                        }
                                    },
                                    {
                                        key: 'user',
                                        label: 'Quản lý thành viên',
                                        icon: <UsergroupAddOutlined />,
                                        onClick: () => {
                                            history.push(`/user/department/team/user/${record.id}`);
                                        }
                                    }
                                ]
                            }}>
                                <Button type="dashed" icon={<MoreOutlined />} size="small" />
                            </Dropdown>,
                            <Popconfirm key={`delete`} title="Bạn có chắc chắn muốn xóa nhóm này?" onConfirm={async () => {
                                await apiTeamDelete(record.id);
                                message.success('Xóa nhóm thành công');
                                actionRef.current?.reload();
                            }}>
                                <Button type="primary" danger icon={<DeleteOutlined />} size="small" />
                            </Popconfirm>
                        ],
                        width: 60
                    }
                ]}
            />
            <ModalForm open={open} onOpenChange={setOpen} title="Nhóm" formRef={formRef} onFinish={onFinish}>
                <ProFormText name="id" hidden />
                <ProFormText name="name" label="Tên nhóm" rules={[{ required: true }]} />
            </ModalForm>
        </PageContainer>
    )
}

export default Index;